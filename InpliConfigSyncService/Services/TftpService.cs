using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace InpliConfigSyncService.Services
{
    public static class TftpService
    {
        public static void Start()
        {
            libtftp.TftpServer.Instance.LogSeverity = libtftp.ETftpLogSeverity.Debug;

            libtftp.TftpServer.Instance.FileReceivedAsync += new Func<object, libtftp.TftpTransferCompleteEventArgs, Task>(
                async (sender, ev) =>
                {
                    ev.Stream.Position = 0;
                    var reader = new StreamReader(ev.Stream);
                    var buffer = reader.ReadToEnd();

                    Console.WriteLine(
                        "Received file from " +
                        ev.RemoteHost.ToString() +
                        " called [" + ev.Filename + "] with " +
                        ev.Stream.Length.ToString() +
                        " bytes"
                    );
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

            libtftp.TftpServer.Instance.GetStream += new Func<object, libtftp.TftpGetStreamEventArgs, Task>(
                async (sender, ev) =>
                {
                    var buffer = await File.ReadAllBytesAsync(@"Sample Data/LorumIpsum.txt");
                    ev.Result = new MemoryStream(buffer);
                }
            );

            libtftp.TftpServer.Instance.Start();
        }
    }
}
