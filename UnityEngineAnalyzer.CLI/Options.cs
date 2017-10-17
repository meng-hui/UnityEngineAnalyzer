using CommandLine;
using System.Collections.Generic;
using UnityEngineAnalyzer.CLI.Reporting;

namespace UnityEngineAnalyzer.CLI
{
    internal class Options
    {
        [ValueOption(0)]
        public string ProjectFile { get; set; }

        [Option('e', "exporter", HelpText = "Exporters to be used.")]
        public IEnumerable<string> Exporters { get; set; }

        [Option('c', "configuration", HelpText = "Custom json configuration to be used.")]
        public string ConfigurationFile { get; set; }

        [Option('s', "severity", DefaultValue = DiagnosticInfo.DiagnosticInfoSeverity.Warning, HelpText = "Minimal severity to be reported.")]
        public DiagnosticInfo.DiagnosticInfoSeverity MinimalSeverity { get; set; }
    }
}