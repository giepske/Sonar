using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Sonar.Logging;

namespace Sonar.Modules.Implementations
{
    class DoSModuleOne : LocalModule
    {
        public override string Name { get; set; } = "DoS Module";
        private List<Denial> serviceDenials = new List<Denial>();
        public override async Task<ModuleResult> Execute(Data data)
        {
            IPAddress hostip = data.GetData<IPAddress>("LocalIpAddress");
            await CompareServiceData(hostip.ToString());
            string denials = "";
            if (!serviceDenials.Any())
            {
                return ModuleResult.Create(this, ResultType.Success, "All hosts' services are up!");
            }
            else
            {
                foreach (Denial d in serviceDenials)
                {
                    denials += Environment.NewLine + d.IP + " - denials - " + d.ServiceDenialed;
                }
                return ModuleResult.Create(this, ResultType.Warning, denials);
            }

        }

        public async Task<List<string>> GetActiveHostsByIP(string hostIP)
        {
            List<string> byPing = new List<string>();
            bool pingable = false;
            string[] separateIP = hostIP.Split('.');
            string subip = separateIP[0] + "." + separateIP[1] + "." + separateIP[2] + ".";

            Logger.Log(LogType.Info, $"Pinging {subip}1 till {subip}255, this might take some time...");

            ConcurrentQueue<int> nrQueue = new ConcurrentQueue<int>(Enumerable.Range(0, 255));

            int threadCount = 255; //we use 1024 threads for the port scanner so 255 threads shouldn't be a problem

            List<Thread> threads = new List<Thread>();

            for (int i = 0; i < threadCount; i++)
            {
                Thread thread = new Thread(() =>
                {
                    while (!nrQueue.IsEmpty)
                    {
                        if (nrQueue.TryDequeue(out int nr))
                        {
                            using (Ping pinger = new Ping())
                            {
                                string tempip = subip + nr;
                                PingReply reply = pinger.Send(tempip, 500);
                                pingable = reply.Status == IPStatus.Success;
                                if (pingable)
                                {
                                    byPing.Add(tempip);
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

            return byPing;
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



        public async Task<List<string>> GetActiveHostsByTCP(string hostIP)
        {
            List<string> byTCP = new List<string>();
            int[] commonports = new int[3] { 25, 80, 443 };
            string[] separateIP = hostIP.Split('.');
            string subip = separateIP[0] + "." + separateIP[1] + "." + separateIP[2] + ".";

            ConcurrentQueue<int> nrQueue = new ConcurrentQueue<int>(Enumerable.Range(0, 255));

            int threadCount = 255; //we use 1024 threads for the port scanner so 255 threads shouldn't be a problem

            List<Thread> threads = new List<Thread>();

            for (int i = 0; i < threadCount; i++)
            {
                Thread thread = new Thread(() =>
                {
                    while (!nrQueue.IsEmpty)
                    {
                        if (nrQueue.TryDequeue(out int nr))
                        {
                            for (int j = 0; j < commonports.Length; j++)
                            {
                                string currip = subip + nr;
                                if (TryConnectTcp(currip, commonports[j]))
                                {
                                    if (!byTCP.Contains(currip))
                                    {
                                        byTCP.Add(currip);
                                    }

                                }
                                else
                                {
                                    if (byTCP.Contains(currip))
                                    {
                                        serviceDenials.Add(
                                            new Denial(currip, " denials tcp on port: " + commonports[j]));
                                    }
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

            return byTCP;
        }




        public async Task CompareServiceData(string hostIP)
        {
            List<string> ipsByPing = await GetActiveHostsByIP(hostIP);
            List<string> ipsByTCP = await GetActiveHostsByTCP(hostIP);

            foreach (string s in ipsByPing)
            {
                if (!ipsByTCP.Contains(s))
                {
                    serviceDenials.Add(new Denial(s, " Accepts ICMP but not TCP"));

                }
            }
            foreach (string s in ipsByTCP)
            {
                if (!ipsByPing.Contains(s))
                {
                    serviceDenials.Add(new Denial(s, "Accepts TCP but not ICMP"));

                }
            }
        }
    }

    class Denial
    {
        public string IP { get; set; }

        public string ServiceDenialed { get; set; }

        public Denial(string ip, string service)
        {
            this.IP = ip;
            this.ServiceDenialed = service;
        }
    }
}

