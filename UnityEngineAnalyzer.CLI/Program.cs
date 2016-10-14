using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace UnityEngineAnalyzer.CLI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                if (args.Length <= 0)
                {
                    return;
                }

                var fileName = args[0];
                var fileInfo = new FileInfo(fileName);

                var report = new AnalyzerReport();
                report.AddExporter(new ConsoleAnalyzerExporter());
                //report.AddExporter(new JsonAnalyzerExporter());

                var tasks = new List<Task>();
                if (fileInfo.Exists)
                {
                    var solutionAnalyzer = new SolutionAnalyzer();
                    var analyzeTask = solutionAnalyzer.LoadAnadAnalyzeProject(fileInfo, report);
                    tasks.Add(analyzeTask);
                }

                Task.WaitAll(tasks.ToArray());

                report.FinalizeReport();

                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
            catch (Exception generalException)
            {
                
                Console.WriteLine("There was an exception running the analysis");
                Console.WriteLine(generalException.ToString());
            }



        }


    }
}
