using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

                if (args.Length > 1 && AvailableExporters.ContainsKey(args[1]))
                {
                    var exporterInstance = Activator.CreateInstance(AvailableExporters[args[1]]);
                    report.AddExporter(exporterInstance as IAnalyzerExporter);
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
                    var solutionAnalyzer = new SolutionAnalyzer();
                    var analyzeTask = solutionAnalyzer.LoadAnadAnalyzeProject(fileInfo, report);
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
