using System;
using System.Collections.Generic;
using System.Text;

namespace Sonar.Modules.Implementations
{
    class DoSModuleOne : LocalModule
    {
        public override string Name { get; set; } = "DoS Module";
        private List<Denial> serviceDenials = new List<Denial>();
        public override Task<ModuleResult> Execute(Data data)
        {
            string hostip = data.GetData<string>("IpAddress");
            CompareServiceData(hostip);
            string denials = "";
            if (!serviceDenials.Any())
            {
                return Task.FromResult(ModuleResult.Create(this, ResultType.Success, "All hosts' services are up!"));
            }
            else
            {
                foreach (Denial d in serviceDenials)
                {
                    denials += Environment.NewLine + d.IP + " - denials - " + d.ServiceDenialed;
                }
                return Task.FromResult(ModuleResult.Create(this, ResultType.Warning, denials));
            }

        }

        public List<string> GetActiveHostsByIP(string hostIP)
        {
            List<string> byPing = new List<string>();
            bool pingable = false;
            string[] separateIP = hostIP.Split('.');
            string subip = separateIP[0] + "." + separateIP[1] + "." + separateIP[2] + ".";
            using (Ping pinger = new Ping())
            {
                for (int i = 1; i < 255; i++)
                {
                    string tempip = subip + i;
                    PingReply reply = pinger.Send(tempip);
                    pingable = reply.Status == IPStatus.Success;
                    if (pingable)
                    {
                        byPing.Add(tempip);
                    }
                }
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



        public List<string> GetActiveHostsByTCP(string hostIP)
        {
            List<string> byTCP = new List<string>();
            int[] commonports = new int[3] { 25, 80, 443 };
            string[] separateIP = hostIP.Split('.');
            string subip = separateIP[0] + "." + separateIP[1] + "." + separateIP[2] + ".";
            for (int i = 1; i < 255; i++)
            {
                for (int j = 0; j < commonports.Length; j++)
                {
                    currip = subip + i;
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
                            serviceDenials.Add(new Denial(currip, " denials tcp on port: " + commonports[j]));
                        }
                    }
                }

            }
            return byTCP;
        }




        public void CompareServiceData(string hostIP)
        {
            List<string> ipsByPing = GetActiveHostsByIP(hostIP);
            List<string> ipsByTCP = GetActiveHostsByTCP(hostIP);



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
        public string IP
        {
            get { return IP; }
            set { IP = value; }
        }

        public string ServiceDenialed
        {
            get { return ServiceDenialed; }
            set { ServiceDenialed = value; }
        }

        public Denial(string ip, string service)
        {
            this.IP = ip;
            this.ServiceDenialed = service;
        }
    }
}

