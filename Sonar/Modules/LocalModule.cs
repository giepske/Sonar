using System.Threading.Tasks;
using Sonar.Logging;

namespace Sonar.Modules
{
    public abstract class LocalModule : IModule
    {
        public abstract string Name { get; set; }
        public abstract Task<ModuleResult> Execute(Data data);
        public ILogger Logger { get; set; }

        protected LocalModule()
        {
        }
    }
}