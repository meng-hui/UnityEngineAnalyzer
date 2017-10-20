using System;

namespace UnityEngineAnalyzer.CLI.Reporting
{
    public abstract class AnalyzerExporter : IAnalyzerExporter
    {
        private readonly Options options;

        public AnalyzerExporter(Options options)
        {
            this.options = options;
        }

        public bool IsAnalyzerRelevant(DiagnosticInfo diagnosticInfo)
        {
            if (options.MinimalSeverity > diagnosticInfo.Severity)
            {
                return false;
            }

            if (options.Version < diagnosticInfo.VersionSpan.First || options.Version > diagnosticInfo.VersionSpan.Last)
            {
                return false;
            }

            return true;
        }

        public abstract void AppendDiagnostic(DiagnosticInfo diagnosticInfo);
        public abstract void FinalizeExporter(TimeSpan duration);
        public abstract void InitializeExporter(Options options);
        public abstract void NotifyException(Exception exception);
    }
}
