using Microsoft.CodeAnalysis.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using UnityEngineAnalyzer.ForEachInUpdate;

namespace UnityEngineAnalyzer.CLI
{
    //NOTE: This kind of configuration file should be created after build
    class ConfigurationFileGenerator
    {
        private const string configurationFileName = "analyzerConfiguration.json";

        public void GenerateConfigurationFile()
        {
            var assembly = typeof(DoNotUseForEachInUpdate).Assembly;
            var allTypes = assembly.DefinedTypes;

            var rootJson = new JObject();

            foreach (var type in allTypes)
            {
                if (type.BaseType == typeof(DiagnosticAnalyzer))
                {
                    rootJson.Add(new JProperty(type.Name, true));
                }
            }

            using (StreamWriter sw = File.CreateText("./" + configurationFileName))
            {
                using (JsonTextWriter writer = new JsonTextWriter(sw))
                {
                    writer.Formatting = Formatting.Indented;
                    rootJson.WriteTo(writer);
                }
            }
        }
    }
}
