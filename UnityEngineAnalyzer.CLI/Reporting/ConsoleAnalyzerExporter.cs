using System;
using System.IO;

namespace UnityEngineAnalyzer.CLI.Reporting
{
    public class ConsoleAnalyzerExporter : IAnalyzerExporter
    {
        private const string ConsoleSeparator = "\t";
        private const DiagnosticInfoSeverity MinimalSeverity = DiagnosticInfoSeverity.Warning;

        public void AppendDiagnostic(DiagnosticInfo diagnosticInfo)
        {
            if (diagnosticInfo.Severity < MinimalSeverity)
            {
                return;
            }

            Console.Write(diagnosticInfo.Id);
            Console.Write(ConsoleSeparator);

            Console.ForegroundColor = ConsoleColorFromSeverity(diagnosticInfo.Severity);
            Console.Write(diagnosticInfo.Severity.ToString());
            Console.Write(ConsoleSeparator);

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(diagnosticInfo.Message);
            Console.ResetColor();
            Console.Write(ConsoleSeparator);
            Console.WriteLine(@"{0}({1})",diagnosticInfo.FileName,diagnosticInfo.LineNumber);
        }

        private ConsoleColor ConsoleColorFromSeverity(DiagnosticInfoSeverity severity)
        {
            switch (severity)
            {
                case DiagnosticInfoSeverity.Hidden:
                    return ConsoleColor.Gray;
                case DiagnosticInfoSeverity.Info:
                    return ConsoleColor.Green;
                case DiagnosticInfoSeverity.Warning:
                    return ConsoleColor.Yellow;
                case DiagnosticInfoSeverity.Error:
                    return ConsoleColor.Red;
                default:
                    return ConsoleColor.White;
            }
        }

        public void Finish(TimeSpan duration)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Console Export Finished ({0})", duration);
            Console.ResetColor();
        }

        public void InitializeExporter(FileInfo projectFile)
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