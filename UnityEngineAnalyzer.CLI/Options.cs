using CommandLine;
using System.Collections.Generic;

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
    }
}