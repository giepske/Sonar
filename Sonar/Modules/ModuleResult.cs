namespace Sonar.Modules
{
    public class ModuleResult
    {
        public readonly string ModuleName;
        public readonly ResultType ResultType;
        public readonly string Message;

        public ModuleResult(string moduleName, ResultType resultType, string message)
        {
            ModuleName = moduleName;
            ResultType = resultType;
            Message = message;
        }

        public static ModuleResult Create(IModule module, ResultType resultType, string message)
        {
            return new ModuleResult(module.Name, resultType, message);
        }
    }
}