using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Net;

namespace Sonar.Modules.Implementations
{
    class DoSModuleTwo : LocalModule
    {
        public override string Name { get; set; } = "DoSModule2 - Network speed";

        public override Task<ModuleResult> Execute(Data data)
        {
            string hostip = data.GetData<string>("IpAddress");
            return Task.FromResult(ModuleResult.Create(this, ResultType.Success, CheckInternetSpeed().ToString()));

        }

        public double CheckInternetSpeed()
        {
            // Create Object Of WebClient
            System.Net.WebClient wc = new System.Net.WebClient();

            //DateTime Variable To Store Download Start Time.
            DateTime dt1 = DateTime.Now;

            //Number Of Bytes Downloaded Are Stored In ‘data’
            byte[] data = wc.DownloadData("http://google.com");

            //DateTime Variable To Store Download End Time.
            DateTime dt2 = DateTime.Now;

            //To Calculate Speed in Kb Divide Value Of data by 1024 And Then by End Time Subtract Start Time To Know Download Per Second.
            return Math.Round((data.Length / 1024) / (dt2 - dt1).TotalSeconds, 2);
        }
    }
}

