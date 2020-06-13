using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Sonar.Logging;

namespace Sonar.Modules.Implementations
{
    public class PortScanModule : LocalModule
    {
        public override string Name { get; set; } = "PortScan Module";

        public override async Task<ModuleResult> Execute(Data data)
        {
            string ipAddress = data.GetData<string>("IpAddress");
            
            Logger.Log(LogType.Info, $"Starting port scan (TCP) on {ipAddress}...");
            Logger.Log(LogType.Info, $""); //empty line to show "... ports left." later

            ConcurrentBag<int> openTcpPorts = new ConcurrentBag<int>();

            ConcurrentQueue<int> portQueue = new ConcurrentQueue<int>(Enumerable.Range(0, 65535));

            int threadCount = 1024;

            List<Thread> threads = new List<Thread>();

            int leftCursorPosition = $"[{LogType.Info}] ".Length;
            int topCursorPosition = Console.CursorTop - 1;

            object lockObject = new object();

            for (int j = 0; j < threadCount; j++)
            {
                Thread thread = new Thread(() =>
                {
                    while (!portQueue.IsEmpty)
                    {
                        if (portQueue.TryDequeue(out int port))
                        {
                            if (TryConnectTcp(ipAddress, port))
                            {
                                openTcpPorts.Add(port);
                            }

                            if (portQueue.Count % 100 == 0)
                            {
                                lock (lockObject)
                                {
                                    Console.SetCursorPosition(leftCursorPosition, topCursorPosition);
                                    Console.Write($"{portQueue.Count} ports left.");
                                }
                            }
                        }
                    }
                });

                thread.IsBackground = true;
                thread.Start();
                threads.Add(thread);
            }

            while (threads.Any(thread => thread.IsAlive))
            {
                await Task.Delay(1000);
            }

            Console.WriteLine(); //make sure the console text coming next will start on a new line

            threads.Clear();

#if DEBUG
            foreach (int openTcpPort in openTcpPorts)
            {
                Logger.Log(LogType.Info, $"Port {openTcpPort} (TCP) is open.");
            }
#endif

            //TODO implement UDP scan that works
            //Logger.Log(LogType.Info, $"Starting port scan (UDP) on {ipAddress}...");

            //List<int> openUdpPorts = new List<int>();

            //for (int j = 0; j < threadCount; j++)
            //{
            //    Thread thread = new Thread(() =>
            //    {
            //        while (!portQueue.IsEmpty)
            //        {
            //            if (portQueue.TryDequeue(out int port))
            //            {
            //                if (TryConnectUdp(ipAddress, port))
            //                {
            //                    openUdpPorts.Add(port);
            //                }
            //            }
            //        }
            //    });

            //    thread.IsBackground = true;
            //    thread.Start();
            //    threads.Add(thread);
            //}

            //while (threads.Any(thread => thread.IsAlive))
            //{
            //    await Task.Delay(1000);
            //}

            //foreach (int openUdpPort in openUdpPorts)
            //{
            //    Logger.Log(LogType.Info, $"Port {openUdpPort} (UDP) is open.");
            //}

            if(openTcpPorts.Count > 0)
                return ModuleResult.Create(this, ResultType.Error, $"Found some open ports on {ipAddress}:\n\r" + 
                    string.Join(" (TCP) is open!" + Environment.NewLine, openTcpPorts) + " (TCP) is open!");

            return ModuleResult.Create(this, ResultType.Success, $"No open ports found on {ipAddress}!");
        }

        private bool TryConnectTcp(string ipAddress, int port)
        {
            using (TcpClient tcpClient = new TcpClient())
            {
                try
                {
                    if (tcpClient.ConnectAsync(ipAddress, port).Wait(2000))
                    {
                        return true;
                    }
                }
                catch (Exception)
                {
                    // ignored
                }

                return false;
            }
        }

        private bool TryConnectUdp(string ipAddress, int port)
        {
            using (UdpClient udpClient = new UdpClient())
            {
                try
                {
                    udpClient.Client.ReceiveTimeout = 2000;

                    udpClient.Connect(ipAddress, port);

                    udpClient.Send(new byte[1], 1);
                    var result = udpClient.ReceiveAsync().Result;

                    return result != null;
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            return false;
        }
    }
}