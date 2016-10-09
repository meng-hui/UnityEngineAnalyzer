using System;
using System.IO;

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

            if (fileInfo.Exists)
            {
                var solutionAnalyzer = new SolutionAnalyzer();
                solutionAnalyzer.LoadAnadAnalyzeProject(fileInfo);
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
