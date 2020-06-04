using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sonar.Modules.Implementations
{
    class DoSModule2 : LocalModule
    {
        public override string Name { get; set; } = "DoSModule2 - Network speed";

        public override Task<ModuleResult> Execute(Data data)
        {
            string hostip = data.GetData<string>("IpAddress");

        }
    }
}
