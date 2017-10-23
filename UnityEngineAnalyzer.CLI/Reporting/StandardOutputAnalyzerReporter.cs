using System;
using static UnityEngineAnalyzer.CLI.Reporting.DiagnosticInfo;

namespace UnityEngineAnalyzer.CLI.Reporting
{
    public class StandardOutputAnalyzerReporter : AnalyzerExporter
    {
        protected const string ConsoleSeparator = "\t";
        protected const string FailurePrefix = "# ";

        public StandardOutputAnalyzerReporter(Options options) : base(options)
        {
        }

        public override void AppendDiagnostic(DiagnosticInfo diagnosticInfo)
        {
            if (IsAnalyzerRelevant(diagnosticInfo) == false)
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

        public override void NotifyException(Exception exception)
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

        public override void FinalizeExporter(TimeSpan duration)
        {
        }

        public override void InitializeExporter(Options options)
        {
        }
    }
}
