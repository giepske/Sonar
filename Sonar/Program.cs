using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sonar.Initialization;
using Sonar.Modules;

namespace Sonar
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Getting initializers...");
            var initializers = GetInitializers();

            Data data = GetData(initializers);

            //todo choose which modules to get and execute
            Console.WriteLine("Getting modules...");
            var modules = GetWebServerModules();

            //todo get host from console
            var results = ExecuteWebServerModules(modules, data, "https://example.com/");

            Console.WriteLine($"Got {results.Count()} results!");

            //todo better logging using logger (and coloring)
            foreach (ModuleResult result in results)
            {
                Console.WriteLine($"[{result.ModuleName}] [{result.ResultType}] {result.Message}");
            }

            Console.ReadLine();
        }

        private static IEnumerable<ModuleResult> ExecuteLocalModules(IEnumerable<Type> modules, Data data)
        {
            foreach (Type moduleType in modules)
            {
                var module = (IModule)Activator.CreateInstance(moduleType);
                yield return module.Execute(data);
            }
        }

        private static IEnumerable<ModuleResult> ExecuteWebServerModules(IEnumerable<Type> modules, Data data, string host)
        {
            foreach (Type moduleType in modules)
            {
                var module = (IModule)Activator.CreateInstance(moduleType, new object?[]
                {
                    host
                });
                yield return module.Execute(data);
            }
        }

        private static IEnumerable<Type> GetLocalModules()
        {
            var localModules = Assembly.GetExecutingAssembly().GetTypes().Where(t =>
                t.BaseType == typeof(LocalModule));

            return localModules;
        }

        private static IEnumerable<Type> GetWebServerModules()
        {
            var webServerModules = Assembly.GetExecutingAssembly().GetTypes().Where(t =>
                t.BaseType == typeof(WebServerModule));

            return webServerModules;
        }

        public static IEnumerable<Type> GetInitializers()
        {
            var initializers = Assembly.GetExecutingAssembly().GetTypes().Where(t =>
                t.BaseType == typeof(Initializer));

            return initializers;
        }

        private static Data GetData(IEnumerable<Type> initializers)
        {
            DataBuilder dataBuilder = new DataBuilder();

            foreach (Type initializerType in initializers)
            {
                var initializer = (Initializer)Activator.CreateInstance(initializerType);
                initializer.SetData(dataBuilder);
            }

            return dataBuilder.Build();
        }
    }
}
