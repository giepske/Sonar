using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Sonar.Initialization;
using Sonar.Logging;
using Sonar.Logging.Implementations;
using Sonar.Modules;

namespace Sonar
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Getting initializers...");
            var initializers = GetInitializers();

            Data data = GetData(initializers);

            int scanType = GetScanType();

            string host = null;

            if (scanType == 2)
                host = GetHost();

            Console.WriteLine("Getting modules...");
            var modules = scanType == 1 ? GetLocalModules() : GetWebServerModules();

            List<ModuleResult> results = null;

            Console.WriteLine("Starting scan...");
            if (scanType == 1)
            {
                results = await ExecuteLocalModules<ConsoleLogger>(modules, data);
            }
            else if(scanType == 2)
            {
                results = await ExecuteWebServerModules<ConsoleLogger>(modules, data, host);
            }

            Console.WriteLine("");
            Console.WriteLine("Scan finished, here are the results:");

            foreach (ModuleResult result in results)
            {
                Log(result);
            }

            Console.WriteLine("Done, press enter to exit...");
            Console.ReadLine();
        }

        public static void Log(ModuleResult moduleResult)
        {
            Console.Write("[");

            SetConsoleColor(ConsoleColor.Cyan);

            Console.Write($"{moduleResult.ModuleName}");

            SetConsoleColor(ConsoleColor.White);

            Console.Write("] [");

            if (moduleResult.ResultType == ResultType.Success)
            {
                SetConsoleColor(ConsoleColor.Green);
            }
            else if (moduleResult.ResultType == ResultType.Warning)
            {
                SetConsoleColor(ConsoleColor.DarkYellow);
            }
            else if (moduleResult.ResultType == ResultType.Error)
            {
                SetConsoleColor(ConsoleColor.Red);
            }

            Console.Write($"{moduleResult.ResultType}");

            SetConsoleColor(ConsoleColor.White);

            Console.WriteLine($"] {moduleResult.Message}");
        }

        private static void SetConsoleColor(ConsoleColor textColor)
        {
            Console.ForegroundColor = textColor;
        }

        private static int GetScanType()
        {
            Console.WriteLine("Sonar has 2 scan types, one for your machine/network and one for a host or domain.");
            Console.WriteLine("Both scan types execute different modules.");
            Console.WriteLine("Please press the number for which scan you want to execute and press enter:");
            Console.WriteLine("1. Local scan (your machine/network)");
            Console.WriteLine("2. WebServer scan (a certain host/domain)");

            bool validScan = false;

            string number;

            do
            {
                number = Console.ReadLine();

                if (int.TryParse(number, out int scanType))
                {
                    validScan = scanType == 1 || scanType == 2;
                }

                if (!validScan)
                {
                    Console.WriteLine("Incorrect scan type, please type the number of the scan you want to execute and press enter:");
                }
            } while (!validScan);

            return int.Parse(number);
        }

        private static string GetHost()
        {
            Console.WriteLine("In order for sonar to do a WebServer scan, we will need an ip address or domain.");
            Console.WriteLine("Please specify a valid ip address or domain and press enter:");

            do
            {
                string host = Console.ReadLine();

                if (IPAddress.TryParse(host, out IPAddress ipAddress))
                {
                    return ipAddress.ToString();
                }

                try
                {
                    ipAddress = Dns.GetHostAddresses(host)[0];

                    return ipAddress.ToString();
                }
                catch (Exception)
                {
                    //ignore
                }

                Console.WriteLine("Incorrect host, please specify a valid ip address or domain and press enter:");
            } while (true);
        }

        private static async Task<List<ModuleResult>> ExecuteLocalModules<TLogger>(IEnumerable<Type> modules, Data data) where TLogger : ILogger, new()
        {
            List<ModuleResult> results = new List<ModuleResult>();

            foreach (Type moduleType in modules)
            {
                var module = (IModule)Activator.CreateInstance(moduleType);

                module.Logger = new TLogger();

                results.Add(await module.Execute(data));
            }

            return results;
        }

        private static async Task<List<ModuleResult>> ExecuteWebServerModules<TLogger>(IEnumerable<Type> modules, Data data, string host) where TLogger : ILogger, new()
        {
            List<ModuleResult> results = new List<ModuleResult>();

            foreach (Type moduleType in modules)
            {
                var module = (IModule)Activator.CreateInstance(moduleType, host);

                module.Logger = new TLogger();

                results.Add(await module.Execute(data));
            }

            return results;
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

        private static IEnumerable<Type> GetInitializers()
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
