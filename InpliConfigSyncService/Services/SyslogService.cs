namespace InpliConfigSyncService.Services
{
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Linq;
    using System.Text;

    public static class SyslogService
    {
        public static void Start(IConfiguration configuration)
        {
            var syslogServerPort = int.Parse(configuration["SyslogServer:Port"]);

            var couchbaseBootstrapServers =
                configuration
                    .GetSection("Couchbase:BootstrapServers")
                    .AsEnumerable()
                    .Where(x => x.Value != null)
                    .Select(x =>
                        new Uri(x.Value)
                    )
                    .ToList();

            var couchbaseBucket = configuration["SyslogServer:Bucket:Name"];
            var couchbaseUsername = configuration["SyslogServer:Bucket:Username"];
            var couchbasePassword = configuration["SyslogServer:Bucket:Password"];
            var documentPrefix = configuration["SyslogServer:DocumentPrefix"];

            SyslogMessageConsumer consumer =
                new SyslogMessageConsumer(
                    couchbaseBootstrapServers,
                    couchbaseBucket,
                    couchbaseUsername,
                    couchbasePassword,
                    documentPrefix
                    );

            void MessageReceivedHandler(object sender, libsyslog.SyslogMessageEvent ev)
            {
                consumer.Consume(ev.Message);
            }

            void UnhandledMessageReceivedHandler(object sender, libsyslog.SyslogParsingErrorEvent ev)
            {
                var oldColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failed to parse message: " + ev.ExceptionMessage);
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(ev.MessageSource.Address.ToString() + " - " + Encoding.UTF8.GetString(ev.MessageData));
                Console.ForegroundColor = oldColor;
            }

            libsyslog.SyslogServer.Instance.MessageReceived += MessageReceivedHandler;
            libsyslog.SyslogServer.Instance.UnhandledMessageReceived += UnhandledMessageReceivedHandler;

            libsyslog.SyslogServer.Instance.Start(syslogServerPort);
        }
    }
}
