using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Net;
using System.Diagnostics;

namespace Sonar.Modules.Implementations
{
    class DoSModuleTwo : LocalModule
    {
        public override string Name { get; set; } = "DoSModule2 - Network speed";

        public override Task<ModuleResult> Execute(Data data)
        {
            string hostip = data.GetData<string>("IpAddress");
            return Task.FromResult(ModuleResult.Create(this, ResultType.Success, CheckInternetSpeed().ToString() + " kb/s"));

        }

        public double CheckInternetSpeed()
        {
            // Create Object Of WebClient
            System.Net.WebClient wc = new System.Net.WebClient();

            //Stopwatch  Variable To Store Download Start Time and End Time.
            var watch = new Stopwatch();

            //Number Of Bytes Downloaded Are Stored In ‘data’
            watch.Start();
            byte[] data = wc.DownloadData("http://dl.google.com/googletalk/googletalk-setup.exe?t=" + DateTime.Now.Ticks);
            watch.Stop();


            //////To Calculate Speed in Kb Divide Value Of data by 1024 And Then by End Time Subtract Start Time To Know Download Per Second.
            return Math.Round((data.LongLength / 1024) / watch.Elapsed.TotalSeconds, 2);
        }
    }
}

