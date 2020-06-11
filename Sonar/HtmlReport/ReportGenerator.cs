using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Sonar.Modules;

namespace Sonar.HtmlReport
{
    public class ReportGenerator
    {
        public string ResultToHtmlTable(string moduleName, string result)
        {
            string table = $@"
            <h2>{ moduleName }</h2>            

            <table class=""table"">";

            List<string> results = result.Split(Environment.NewLine).ToList();

            foreach (var item in results)
            { 
                string tableRow = $@"
                <tr>
                    <td>{item}</td>
                </tr>";

                table += tableRow;
            }

            table += "</table>";

            return table;
        }

        public void GenerateReport(List<ModuleResult> results)
        {
            string tables = null;

            results.ForEach(result =>
            {
                tables += ResultToHtmlTable(result.ModuleName, result.Message);
            });

            string style = File.ReadAllText("CssTemplate.css");
            string html = $@"
            
            <html>
            <head>
            <link rel=""stylesheet"" href=""https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css"" integrity=""sha384-ggOyR0iXCbMQv3Xipma34MD+dH/1fQ784/j6cY/iJTQUOhcWr7x9JvoRxT2MZw1T"" crossorigin=""anonymous"">
            </ head>
            <body class=""text-center"">
            
            <img id=""logo"" src=""SonarLogo.svg"" />
            
            {tables}
            </body>
            <style>{style}</style>
            </html>";

            string directory = $"{Directory.GetCurrentDirectory()}/report.html";
            File.WriteAllText(directory, html);
        }
    }
}
