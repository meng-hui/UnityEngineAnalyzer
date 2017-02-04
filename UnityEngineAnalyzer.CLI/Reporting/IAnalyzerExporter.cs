using System;
using System.IO;

namespace UnityEngineAnalyzer.CLI.Reporting
{
    public interface IAnalyzerExporter
    {
        void AppendDiagnostic(DiagnosticInfo diagnosticInfo);
        void FinalizeExporter(TimeSpan duration);
        void InitializeExporter(FileInfo projectFile);
        void NotifyException(Exception exception);
    }
}