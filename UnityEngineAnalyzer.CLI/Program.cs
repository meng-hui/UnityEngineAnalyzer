using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngineAnalyzer.CLI.Reporting;

namespace UnityEngineAnalyzer.CLI
{
    public class Program
    {
        private static readonly Dictionary<string, Type> AvailableExporters = new Dictionary<string, Type>();

        static Program()
        {
            AvailableExporters.Add(nameof(JsonAnalyzerExporter), typeof(JsonAnalyzerExporter));
            AvailableExporters.Add(nameof(StandardOutputAnalyzerReporter), typeof(StandardOutputAnalyzerReporter));
            AvailableExporters.Add(nameof(ConsoleAnalyzerExporter), typeof(ConsoleAnalyzerExporter));
        }


        public static void Main(string[] args)
        {
            try
            {
                //TODO: Use a proper parser for the commands

                if (args.Length <= 0)
                {
                    return;
                }

                var startTime = DateTime.Now;

                var fileName = args[0];
                var fileInfo = new FileInfo(fileName);

                //NOTE: This could be configurable via the CLI at some point
                var report = new AnalyzerReport();

                int optinalArgumentIndex = 1;
                if (args.Length > optinalArgumentIndex && AvailableExporters.ContainsKey(args[optinalArgumentIndex]))
                {
                    var exporterInstance = Activator.CreateInstance(AvailableExporters[args[optinalArgumentIndex]]);
                    report.AddExporter(exporterInstance as IAnalyzerExporter);

                    optinalArgumentIndex++;
                }
                else
                {
                    //It's generally a good idea to make sure that the Console Exporter is last since it is interactive
                    report.AddExporter(new JsonAnalyzerExporter());
                    report.AddExporter(new ConsoleAnalyzerExporter());
                }
                

                report.InitializeReport(fileInfo);

                var tasks = new List<Task>();
                if (fileInfo.Exists)
                {
                    FileInfo configFileInfo = null;
                    if (args.Length > optinalArgumentIndex)
                    {
                        var configFileName = args[optinalArgumentIndex];
                        configFileInfo = new FileInfo(configFileName);
                    }

                    var solutionAnalyzer = new SolutionAnalyzer();
                    var analyzeTask = solutionAnalyzer.LoadAndAnalyzeProjectAsync(fileInfo, configFileInfo, report);
                    tasks.Add(analyzeTask);
                }

                Task.WaitAll(tasks.ToArray());

                var endTime = DateTime.Now;
                var duration = endTime - startTime;

                report.FinalizeReport(duration);

            }
            catch (Exception generalException)
            {
                
                Console.WriteLine("There was an exception running the analysis");
                Console.WriteLine(generalException.ToString());
            }
        }


    }
}
