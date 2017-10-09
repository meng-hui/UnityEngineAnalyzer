using System;
using System.IO;

namespace UnityEngineAnalyzer.CLI.Reporting
{
    public class ConsoleAnalyzerExporter : StandardOutputAnalyzerReporter
    {

        public override void FinalizeExporter(TimeSpan duration)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Console Export Finished ({0})", duration);
            Console.ResetColor();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        public override void InitializeExporter(FileInfo projectFile)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Unity Syntax Analyzer");
            Console.WriteLine();
            Console.WriteLine("Analyzing: {0}", projectFile.FullName);
            Console.WriteLine();
            Console.ResetColor();
        }
    }
}