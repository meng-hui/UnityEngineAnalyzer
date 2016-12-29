using System;
using System.IO;

namespace UnityEngineAnalyzer.CLI.Reporting
{
    public class StandardOutputAnalyzerReporter : IAnalyzerExporter
    {
        protected const string ConsoleSeparator = "\t";
        protected const DiagnosticInfo.DiagnosticInfoSeverity MinimalSeverity = DiagnosticInfo.DiagnosticInfoSeverity.Warning;

        protected const string FailurePrefix = "# ";

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
            Console.WriteLine(@"{0}{1}{0}{2},{3}", ConsoleSeparator,diagnosticInfo.FileName, diagnosticInfo.LineNumber, diagnosticInfo.CharacterPosition);
            
        }

        private ConsoleColor ConsoleColorFromSeverity(DiagnosticInfo.DiagnosticInfoSeverity severity)
        {
            switch (severity)
            {
                case DiagnosticInfo.DiagnosticInfoSeverity.Hidden:
                    return ConsoleColor.Gray;
                case DiagnosticInfo.DiagnosticInfoSeverity.Info:
                    return ConsoleColor.Green;
                case DiagnosticInfo.DiagnosticInfoSeverity.Warning:
                    return ConsoleColor.Yellow;
                case DiagnosticInfo.DiagnosticInfoSeverity.Error:
                    return ConsoleColor.Red;
                default:
                    return ConsoleColor.White;
            }
        }

        public virtual void FinalizeExporter(TimeSpan duration)
        {
        }

        public virtual void InitializeExporter(FileInfo projectFile)
        {
        }

        public virtual void NotifyException(Exception exception)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            var delimeters = new[] {"\r", "\n", Environment.NewLine };
            var exceptionLines = exception.ToString().Split(delimeters, StringSplitOptions.RemoveEmptyEntries);

            foreach (var exceptionLine in exceptionLines)
            {
                Console.WriteLine(FailurePrefix + exceptionLine);
            }

            Console.WriteLine(FailurePrefix);

            Console.ResetColor();
        }
    }
}
