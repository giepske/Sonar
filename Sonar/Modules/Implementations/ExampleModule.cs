using System;
using System.Threading.Tasks;

namespace Sonar.Modules.Implementations
{
    public class ExampleModule : WebServerModule
    {
        public override string Name { get; set; } = "Example Module";

        public ExampleModule(string host) : base(host)
        {
        }

        public override Task<ModuleResult> Execute(Data data)
        {
            Uri uri = new Uri(Host);

            if(Host.StartsWith("https://") || uri.Port == 443)
                return Task.FromResult(ModuleResult.Create(this, ResultType.Success, "Host is HTTPS!"));
            if (Host.StartsWith("http://") || uri.Port == 80)
                return Task.FromResult(ModuleResult.Create(this, ResultType.Error, "Host is HTTP!"));

            return Task.FromResult(ModuleResult.Create(this, ResultType.Error, "Unable to see if host is HTTPS."));
        }
    }
}
