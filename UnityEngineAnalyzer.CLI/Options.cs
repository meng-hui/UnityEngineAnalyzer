using CommandLine;
using System.Collections.Generic;
using static UnityEngineAnalyzer.CLI.Reporting.DiagnosticInfo;

namespace UnityEngineAnalyzer.CLI
{
    public class Options
    {
        [ValueOption(0)]
        public string ProjectFile { get; set; }

        [Option('e', "exporter", HelpText = "Exporters to be used.")]
        public IEnumerable<string> Exporters { get; set; }

        [Option('c', "configuration", HelpText = "Custom json configuration to be used.")]
        public string ConfigurationFile { get; set; }

        [Option('s', "severity", DefaultValue = DiagnosticInfoSeverity.Warning, HelpText = "Minimal severity to be reported.")]
        public DiagnosticInfoSeverity MinimalSeverity { get; set; }

        [Option('v', "version", DefaultValue = UnityVersion.NONE, HelpText = "Check against spesific Unity version.")]
        public UnityVersion Version { get; set; }
    }
}