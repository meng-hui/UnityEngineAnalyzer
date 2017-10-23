﻿using System;
using System.Collections.Generic;
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

        public int GetExporterCount()
        {
            return _exporters.Count;
        }

        public void AppendDiagnostics(IEnumerable<Diagnostic> diagnosticResults)
        {
            if (_exporters.Count == 0)
            {
                return;
            }

            foreach (var diagnostic in diagnosticResults)
            {
                var location = diagnostic.Location;
                var lineNumber = 0;
                var characterPosition = 0;
                var fileName = string.Empty;

                if (location != Location.None)
                {
                    var locationSpan = location.SourceSpan;
                    var lineSpan = location.SourceTree.GetLineSpan(locationSpan);
                    lineNumber = lineSpan.StartLinePosition.Line;
                    characterPosition = lineSpan.StartLinePosition.Character;
                    fileName = location.SourceTree?.FilePath;
                }

                var diagnosticInfo = new DiagnosticInfo
                {
                    Id = diagnostic.Id,
                    Message = diagnostic.GetMessage(),
                    FileName = fileName,
                    LineNumber = lineNumber,
                    CharacterPosition = characterPosition,
                    Severity = (DiagnosticInfo.DiagnosticInfoSeverity)diagnostic.Severity,
                    VersionSpan = DiagnosticDescriptors.GetVersion(diagnostic.Descriptor)
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
                exporter.FinalizeExporter(duration);
            }
        }

        public void InitializeReport(Options options)
        {
            foreach (var exporter in _exporters)
            {
                exporter.InitializeExporter(options);
            }
        }

        public void NotifyException(Exception exception)
        {
            foreach (var exporter in _exporters)
            {
                exporter.NotifyException(exception);
            }
        }
    }
}
