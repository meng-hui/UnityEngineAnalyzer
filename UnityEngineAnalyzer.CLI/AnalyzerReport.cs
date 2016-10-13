using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace UnityEngineAnalyzer.CLI
{
    public class AnalyzerReport
    {
        public string ProjectName { get; set; }

        //TODO: Add support for Solutions with multiple Projects
        private readonly List<DiagnosticInfo> _diagnostics = new List<DiagnosticInfo>();

        private readonly List<IAnalyzerExporter> _exporters = new List<IAnalyzerExporter>();

        public void AppendDiagnostic(Diagnostic diagnostic)
        {
            var diagnosticInfo = new DiagnosticInfo
            {
                Id = diagnostic.Id,
                Message = diagnostic.GetMessage()
            };

            _diagnostics.Add(diagnosticInfo);
        }

        public void AddExporter(IAnalyzerExporter exporter)
        {
            _exporters.Add(exporter);
        }

        public void Export(params IAnalyzerExporter[] exporters)
        {
            _exporters.AddRange(exporters);
            this.Export();
        }
        public void Export()
        {
            
        }

        public void AppendDiagnostics(IEnumerable<Diagnostic> diagnosticResults)
        {
            if (_exporters.Count == 0)
            {
                return;
            }

            foreach (var diagnostic in diagnosticResults)
            {
                var diagnosticInfo = new DiagnosticInfo
                {
                    Id = diagnostic.Id,
                    Message = diagnostic.GetMessage()
                };

                foreach (var exporter in _exporters)
                {
                    exporter.AppendDiagnostic(diagnosticInfo);
                }
            }
        }

        public void FinalizeReport()
        {
            foreach (var exporter in _exporters)
            {
                exporter.Finish();
            }
        }
    }

    public interface IAnalyzerExporter
    {
        void AppendDiagnostic(DiagnosticInfo diagnosticInfo);
        void Finish();
    }

    public class DiagnosticInfo
    {
        public string Id { get; set; }
        public string Message { get; set; }
        public string FileName { get; set; }

    }

    public class JsonAnalyzerExporter : IAnalyzerExporter
    {
        private List<DiagnosticInfo> _diagnostics = new List<DiagnosticInfo>();

        public void AppendDiagnostic(DiagnosticInfo diagnosticInfo)
        {
            _diagnostics.Add(diagnosticInfo);
        }

        public void Finish()
        {
            Console.WriteLine("This is where we write the json file" + _diagnostics.Count);
        }
    }

    public class ConsoleAnalyzerExporter : IAnalyzerExporter
    {
        public void AppendDiagnostic(DiagnosticInfo diagnosticInfo)
        {
            Console.WriteLine(diagnosticInfo.Message);
        }

        public void Finish()
        {
            Console.WriteLine("FINISHED");
        }
    }


}
