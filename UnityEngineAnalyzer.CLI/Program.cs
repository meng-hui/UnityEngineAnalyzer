using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace UnityEngineAnalyzer.CLI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length <= 0)
            {
                return;
            }

            var fileName = args[0];
            var fileInfo = new FileInfo(fileName);


            var tasks = new List<Task>();
            if (fileInfo.Exists)
            {
                var solutionAnalyzer = new SolutionAnalyzer();
                var analyzeTask = solutionAnalyzer.LoadAnadAnalyzeProject(fileInfo);
                tasks.Add(analyzeTask);
            }

            Task.WaitAll(tasks.ToArray());


            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }


    }
}
