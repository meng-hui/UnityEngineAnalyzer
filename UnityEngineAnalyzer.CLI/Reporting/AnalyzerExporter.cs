using System;
using System.IO;

namespace UnityEngineAnalyzer.CLI.Reporting
{
    public abstract class AnalyzerExporter : IAnalyzerExporter
    {
        //NOTE: Can we use using.static.DiagnosticsInfo; C#6 feature?
        protected DiagnosticInfo.DiagnosticInfoSeverity MinimalSeverity;

        public AnalyzerExporter(DiagnosticInfo.DiagnosticInfoSeverity MinimalSeverity)
        {
            this.MinimalSeverity = MinimalSeverity;
        }

        public abstract void AppendDiagnostic(DiagnosticInfo diagnosticInfo);
        public abstract void FinalizeExporter(TimeSpan duration);
        public abstract void InitializeExporter(FileInfo projectFile);
        public abstract void NotifyException(Exception exception);
    }
}
