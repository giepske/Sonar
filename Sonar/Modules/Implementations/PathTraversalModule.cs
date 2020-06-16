using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sonar.Modules.Implementations
{
    public class PathTraversalModule : WebServerModule
    {
        private readonly string _host;
        private ConcurrentBag<string> _foundHost = new ConcurrentBag<string>();

        public PathTraversalModule(string host) : base(host)
        {
            _host = host;
        }

        public override string Name { get; set; } = "Path traversal";

        private async Task<List<string>> GetPathTraversalList(string url)
        {
            var response = await new HttpClient().GetAsync(url);
            var result = await response.Content.ReadAsStringAsync();

            List<string> pathList = result.Split(new[] { "\n" }, StringSplitOptions.None).ToList();

            return pathList;
        }

        private async Task FindDirectory(string url)
        {
            using var httpClient = new HttpClient();

            try
            {
                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode && _foundHost.All(path => path != url))
                    _foundHost.Add(url);
            }

            catch (Exception) { /* ignore to prevent cancellation */ }
        }

        private void ExecutePathTraversal(string url, List<string> directories)
        {
            var tasks = new List<Task>();

            foreach (var directory in directories)
                tasks.Add(FindDirectory(url + directory));

            Task.WaitAll(tasks.ToArray());
        }

        public override async Task<ModuleResult> Execute(Data data)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var pathList = await GetPathTraversalList("https://raw.githubusercontent.com/Bo0oM/fuzz.txt/master/fuzz.txt");
            ExecutePathTraversal(_host, pathList);
            var result = string.Join(Environment.NewLine, _foundHost);

            stopWatch.Stop();
            result += $"{ Environment.NewLine } Pathtraversal completed in: { stopWatch.Elapsed.TotalSeconds } seconds";

            return ModuleResult.Create(this, ResultType.Success, result);
        }
    }
}