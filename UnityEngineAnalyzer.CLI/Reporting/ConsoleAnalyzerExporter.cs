using System;

namespace UnityEngineAnalyzer.CLI.Reporting
{
    public class ConsoleAnalyzerExporter : StandardOutputAnalyzerReporter
    {
        public ConsoleAnalyzerExporter(Options options) : base(options)
        {
        }

        public override void FinalizeExporter(TimeSpan duration)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Console Export Finished ({0})", duration);
            Console.ResetColor();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        public override void InitializeExporter(Options options)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Unity Syntax Analyzer");
            Console.WriteLine();
            Console.WriteLine("Analyzing: {0}", options.ProjectFile);
            Console.WriteLine("With Unity version: " + Enum.GetName((typeof(UnityVersion)), options.Version));
            Console.WriteLine();
            Console.ResetColor();
        }
    }
}