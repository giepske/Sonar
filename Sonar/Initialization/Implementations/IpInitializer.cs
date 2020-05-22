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
                string ip = await httpClient.GetStringAsync("https://icanhazip.com/");

                dataBuilder.SetData("IpAddress", ip);
            }
        }
    }
}
