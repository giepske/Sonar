using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Linq;
using System.Net.Sockets;

namespace Sonar.Modules.Implementations
{
    class DoSModule : LocalModule
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

        public List<string> GetActiveHostsByTCP(string hostIP)
        {
            List<string> byTCP = new List<string>();
            int[] commonports = new int[3] { 25,80,443 };
            string[] separateIP = hostIP.Split('.');
            string subip = separateIP[0] + "." + separateIP[1] + "." + separateIP[2] + ".";
            for (int i = 1; i < 255; i++)
            {
                for(int j = 0; j < commonports.Length; j++)
                {
                    try
                    {
                        Int32 port = commonports[j];
                        string currip = subip + i;
                        TcpClient client = new TcpClient(currip, port);
                        Byte[] data = System.Text.Encoding.ASCII.GetBytes("Test");
                        NetworkStream stream = client.GetStream();
                        stream.Write(data, 0, data.Length);
                        data = new Byte[256];
                        String responseData = String.Empty;
                        Int32 bytes = stream.Read(data, 0, data.Length);
                        responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

                        if (responseData != String.Empty)
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
                                serviceDenials.Add(new Denial(currip, "TCP - port - " + port));
                            }
                        }
                        stream.Close();
                        client.Close();
                    }
                    catch (ArgumentNullException e)
                    {

                    }
                    catch (SocketException e)
                    {

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
                    serviceDenials.Add(new Denial(s, "Reacts to ICMP but not to TCP"));

                }
            }
            foreach (string s in ipsByTCP)
            {
                if (!ipsByPing.Contains(s))
                {
                    serviceDenials.Add(new Denial(s, "Reacts to TCP but not to ICMP"));

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
