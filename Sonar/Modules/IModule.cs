using System.Threading.Tasks;
using Sonar.Logging;

namespace Sonar.Modules
{
    public interface IModule
    {
        public string Name { get; set; }
        public Task<ModuleResult> Execute(Data data);
        public ILogger Logger { get; set; }
    }
}
