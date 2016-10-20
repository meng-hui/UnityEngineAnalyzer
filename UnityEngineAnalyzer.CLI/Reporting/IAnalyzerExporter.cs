using System;

namespace UnityEngineAnalyzer.CLI.Reporting
{
    public interface IAnalyzerExporter
    {
        void AppendDiagnostic(DiagnosticInfo diagnosticInfo);
        void Finish(TimeSpan duration);
        void InitializeExporter(string fileName);
    }
}