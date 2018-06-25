namespace InpliConfigSyncService.Services
{
    using Couchbase;
    using Couchbase.Authentication;
    using Couchbase.Configuration.Client;
    using Couchbase.Core.Serialization;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public static class TftpService
    {
        public class ConfigurationDocument
        {
            public string Type { get; set; }
            public string Configuration { get; set; }
        }

        public static void Start(IConfiguration configuration)
        {
            var tftpServerPort = int.Parse(configuration["TftpServer:Port"]);

            var couchbaseBootstrapServers =
                configuration
                    .GetSection("Couchbase:BootstrapServers")
                    .AsEnumerable()
                    .Where(x => x.Value != null)
                    .Select(x =>
                        new Uri(x.Value)
                    )
                    .ToList();

            var couchbaseBucket = configuration["TftpServer:Bucket:Name"];
            var couchbaseUsername = configuration["TftpServer:Bucket:Username"];
            var couchbasePassword = configuration["TftpServer:Bucket:Password"];
            var receivedDocumentPrefix = configuration["TftpServer:ReceivedDocumentPrefix"];

            var DbCluster = new Cluster(
                new ClientConfiguration
                {
                    Servers = couchbaseBootstrapServers,
                    Serializer = () =>
                    {
                        JsonSerializerSettings serializerSettings =
                            new JsonSerializerSettings()
                            {
                                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
                                DateTimeZoneHandling = DateTimeZoneHandling.Utc
                            };

                        serializerSettings.Converters.Add(new IPAddressConverter());
                        serializerSettings.Converters.Add(new IPEndPointConverter());

                        return new DefaultSerializer(serializerSettings, serializerSettings);
                    }
                }
            );

            var authenticator = new PasswordAuthenticator(couchbaseUsername, couchbasePassword);
            DbCluster.Authenticate(authenticator);

            libtftp.TftpServer.Instance.LogSeverity = libtftp.ETftpLogSeverity.Debug;

            libtftp.TftpServer.Instance.FileReceived +=
                new EventHandler<libtftp.TftpTransferCompleteEventArgs>(async (sender, ev) =>
                {
                    ev.Stream.Position = 0;
                    var reader = new StreamReader(ev.Stream);
                    var buffer = await reader.ReadToEndAsync();

                    Console.WriteLine(
                        "Received file from " +
                        ev.RemoteHost.ToString() +
                        " called [" + ev.Filename + "] with " +
                        ev.Stream.Length.ToString() +
                        " bytes"
                    );

                    using (var bucket = await DbCluster.OpenBucketAsync(couchbaseBucket))
                    {
                        var document = new Document<dynamic>
                        {
                            Id = receivedDocumentPrefix + ev.Filename,
                            Content = new ConfigurationDocument
                            {
                                Type = "tftp::configuration",
                                Configuration = buffer
                            }
                        };

                        var insert = await bucket.InsertAsync(document);
                        if (insert.Success)
                        {
                            Console.WriteLine(document.Id);
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine(insert.Status.ToString());
                        }
                    }
                }
            );

            libtftp.TftpServer.Instance.FileTransmitted +=
                new EventHandler<libtftp.TftpTransferCompleteEventArgs>((sender, ev) =>
                {
                    Console.WriteLine(
                        "Transmitted file to " +
                        ev.RemoteHost.ToString() +
                        " called [" + ev.Filename + "]"
                        );
                }
            );

            libtftp.TftpServer.Instance.Log +=
                new EventHandler<libtftp.TftpLogEventArgs>((sender, ev) =>
                {
                    switch (ev.Severity)
                    {
                        case libtftp.ETftpLogSeverity.Error:
                            Console.ForegroundColor = ConsoleColor.Red;
                            break;

                        case libtftp.ETftpLogSeverity.Debug:
                            Console.ForegroundColor = ConsoleColor.Gray;
                            break;

                        default:
                            Console.ForegroundColor = ConsoleColor.White;
                            break;
                    }

                    Console.Write("[" + ev.TimeStamp.ToString() + "]: ");

                    Console.ForegroundColor = ConsoleColor.White;

                    Console.WriteLine(ev.Message);
                }
            );

            //libtftp.TftpServer.Instance.GetStream += new Func<object, libtftp.TftpGetStreamEventArgs, Task>(
            //    async (sender, ev) =>
            //    {
            //        var buffer = await File.ReadAllBytesAsync(@"Sample Data/LorumIpsum.txt");
            //        ev.Result = new MemoryStream(buffer);
            //    }
            //);

            libtftp.TftpServer.Instance.Start(tftpServerPort);
        }
    }
}
