using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis;
using UnityEngineAnalyzer.CLI.Reporting;

namespace UnityEngineAnalyzer.CLI
{
    public class AnalyzerReport
    {
        //TODO: Add support for Solutions with multiple Projects

        private readonly List<IAnalyzerExporter> _exporters = new List<IAnalyzerExporter>();

        public void AddExporter(IAnalyzerExporter exporter)
        {
            _exporters.Add(exporter);
        }

        public void AppendDiagnostics(IEnumerable<Diagnostic> diagnosticResults)
        {
            if (_exporters.Count == 0)
            {
                return;
            }

            foreach (var diagnostic in diagnosticResults)
            {
                var locationSpan = diagnostic.Location.SourceSpan;
                var lineSpan = diagnostic.Location.SourceTree.GetLineSpan(locationSpan);

                var diagnosticInfo = new DiagnosticInfo
                {
                    Id = diagnostic.Id,
                    Message = diagnostic.GetMessage(),
                    FileName = diagnostic.Location.SourceTree.FilePath,
                    LineNumber = lineSpan.StartLinePosition.Line,
                    Severity = (DiagnosticInfoSeverity)diagnostic.Severity
                };


                foreach (var exporter in _exporters)
                {
                    exporter.AppendDiagnostic(diagnosticInfo);
                }
            }
        }

        public void FinalizeReport(TimeSpan duration)
        {
            foreach (var exporter in _exporters)
            {
                exporter.Finish(duration);
            }
        }

        public void InitializeReport(FileInfo projectFile)
        {
            foreach (var exporter in _exporters)
            {
                exporter.InitializeExporter(projectFile);       
            }
        }
    }
}
