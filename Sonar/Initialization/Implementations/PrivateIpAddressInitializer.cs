using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Linq;
using System.Net.Sockets;
namespace Sonar.Initialization.Implementations
{
    class PrivateIpAddressInitializer : Initializer
    {
        public static string GetLocalIPAddress()
        {
            
        }

        public override async Task SetData(DataBuilder dataBuilder)
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    dataBuilder.SetData("LocalIpAddress", ip);
                }
            }
        }
    }
}
