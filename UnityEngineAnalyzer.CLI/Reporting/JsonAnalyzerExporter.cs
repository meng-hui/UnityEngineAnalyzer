using System;
using System.Collections.Generic;

namespace UnityEngineAnalyzer.CLI.Reporting
{
    public class JsonAnalyzerExporter : IAnalyzerExporter
    {
        private readonly List<DiagnosticInfo> _diagnostics = new List<DiagnosticInfo>();

        public void AppendDiagnostic(DiagnosticInfo diagnosticInfo)
        {
            _diagnostics.Add(diagnosticInfo);
        }

        public void Finish(TimeSpan duration)
        {
            Console.WriteLine("This is where we write the json file : " + _diagnostics.Count);
        }

        public void InitializeExporter(string fileName)
        {
            throw new NotImplementedException();
        }
    }
}