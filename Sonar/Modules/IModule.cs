using Sonar.Logging;

namespace Sonar.Modules
{
    public interface IModule
    {
        public string Name { get; set; }
        public ModuleResult Execute(Data data);
        public ILogger Logger { get; set; }
    }
}
