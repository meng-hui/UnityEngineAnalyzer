using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

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

        public void InitializeReport(string fileInfoName)
        {
            foreach (var exporter in _exporters)
            {
                exporter.InitializeExporter(fileInfoName);       
            }
        }
    }

    public interface IAnalyzerExporter
    {
        void AppendDiagnostic(DiagnosticInfo diagnosticInfo);
        void Finish(TimeSpan duration);
        void InitializeExporter(string fileName);
    }

    public enum DiagnosticInfoSeverity
    {
        Hidden = 0,
        Info = 1,
        Warning = 2,
        Error = 3
    }

    public class DiagnosticInfo
    {
        public string Id { get; set; }
        public string Message { get; set; }
        public string FileName { get; set; }
        public int LineNumber { get; set; }
        public DiagnosticInfoSeverity Severity { get; set; }
    }

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

    public class ConsoleAnalyzerExporter : IAnalyzerExporter
    {
        private const string ConsoleSeparator = "\t";

        public void AppendDiagnostic(DiagnosticInfo diagnosticInfo)
        {
            Console.Write(diagnosticInfo.Id);
            Console.Write(ConsoleSeparator);

            Console.ForegroundColor = ConsoleColorFromSeverity(diagnosticInfo.Severity);
            Console.Write(diagnosticInfo.Severity.ToString());
            Console.Write(ConsoleSeparator);

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(diagnosticInfo.Message);
            Console.ResetColor();
            Console.Write(ConsoleSeparator);
            Console.WriteLine("{0}({1})",diagnosticInfo.FileName,diagnosticInfo.LineNumber);
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

        public void Finish(TimeSpan duration)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Console Export Finished ({0})", duration);
            Console.ResetColor();
        }

        public void InitializeExporter(string fileName)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Unity Syntax Analyzer");
            Console.WriteLine();
            Console.WriteLine("Analyzing: {0}", fileName);
            Console.WriteLine();
            Console.ResetColor();
        }
    }


}
