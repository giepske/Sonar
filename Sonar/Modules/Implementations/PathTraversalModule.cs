using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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

            List<string> pathList = result.Split(new[] { "\r\n" }, StringSplitOptions.None).ToList();
            pathList.RemoveRange(0, 8);

            return pathList;
        }

        private async Task FindDirectory(string url)
        {
            var httpClient = new HttpClient();

            try
            {
                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode && _foundHost.All(path => path != url))
                    _foundHost.Add(url);
            }

            catch (Exception) { }
        }

        private async Task ExecutePathTraversal(string url, List<string> directories)
        {
            var tasks = new List<Task>();

            foreach (var directory in directories)
            {
                tasks.Add(FindDirectory(url + directory));
            }

            Task.WaitAll(tasks.ToArray());
        }

        public override ModuleResult Execute(Data data)
        {
            var pathList = GetPathTraversalList("https://www.vulnerability-lab.com/resources/documents/587.txt").Result;
            ExecutePathTraversal("https://cloud.vdarwinkel.nl", pathList);
            var result = string.Join(Environment.NewLine, _foundHost);

            return ModuleResult.Create(this, ResultType.Error, result);
        }
    }
}