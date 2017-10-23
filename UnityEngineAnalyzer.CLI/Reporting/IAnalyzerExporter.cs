using System;
using System.IO;

namespace UnityEngineAnalyzer.CLI.Reporting
{
    public interface IAnalyzerExporter
    {
        void AppendDiagnostic(DiagnosticInfo diagnosticInfo);
        void FinalizeExporter(TimeSpan duration);
        void InitializeExporter(Options options);
        void NotifyException(Exception exception);
    }
}