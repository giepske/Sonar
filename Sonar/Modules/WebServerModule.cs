using Sonar.Logging;

namespace Sonar.Modules
{
    public abstract class WebServerModule : IModule
    {
        protected readonly string Host;
        public abstract string Name { get; set; }
        public abstract ModuleResult Execute(Data data);
        public ILogger Logger { get; set; }

        protected WebServerModule(string host)
        {
            Host = host;
        }
    }
}