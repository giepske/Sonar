using System.Net.Http;
using System.Threading.Tasks;

namespace Sonar.Initialization.Implementations
{
    public class IpAddressInitializer : Initializer
    {
        public override async Task SetData(DataBuilder dataBuilder)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string ipAddress = await httpClient.GetStringAsync("http://ipv4.icanhazip.com/");

                ipAddress = ipAddress.Trim();

                dataBuilder.SetData("IpAddress", ipAddress);
            }
        }
    }
}
