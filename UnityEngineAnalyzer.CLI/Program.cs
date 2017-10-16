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
                var options = new Options();
                var isValid = CommandLine.Parser.Default.ParseArgumentsStrict(args, options);

                if (isValid == false || options.ProjectFile == null)
                {
                    return;
                }

                var startTime = DateTime.Now;

                var fileName = options.ProjectFile;
                var fileInfo = new FileInfo(fileName);

                //NOTE: This could be configurable via the CLI at some point
                var report = new AnalyzerReport();

                if (options.Exporters != null)
                {
                    foreach (var exporter in options.Exporters)
                    {
                        if (AvailableExporters.ContainsKey(exporter))
                        {
                            var exporterInstance = Activator.CreateInstance(AvailableExporters[exporter]);
                            report.AddExporter(exporterInstance as IAnalyzerExporter);
                        }
                    }
                }

                if (report.GetExporterCount() == 0)
                { 
                    //It's generally a good idea to make sure that the Console Exporter is last since it is interactive
                    report.AddExporter(new JsonAnalyzerExporter(options.MinimalSeverity));
                    report.AddExporter(new ConsoleAnalyzerExporter(options.MinimalSeverity));
                }
                
                report.InitializeReport(fileInfo);

                var tasks = new List<Task>();
                if (fileInfo.Exists)
                {
                    FileInfo configFileInfo = null;

                    if (options.ConfigurationFile != null)
                    {
                        configFileInfo = new FileInfo(options.ConfigurationFile);
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
