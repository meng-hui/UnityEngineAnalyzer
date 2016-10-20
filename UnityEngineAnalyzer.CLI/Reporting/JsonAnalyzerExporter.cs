using System;
using System.IO;
using Newtonsoft.Json;

namespace UnityEngineAnalyzer.CLI.Reporting
{
    public class JsonAnalyzerExporter : IAnalyzerExporter
    {
        private JsonTextWriter _jsonWriter;
        private readonly JsonSerializer _jsonSerializer = new JsonSerializer();

        public void AppendDiagnostic(DiagnosticInfo diagnosticInfo)
        {
            _jsonSerializer.Serialize(_jsonWriter, diagnosticInfo);
        }

        public void Finish(TimeSpan duration)
        {
            _jsonWriter.WriteEndArray();
            _jsonWriter.WriteEndObject();
            _jsonWriter.Close();
        }

        public void InitializeExporter(FileInfo projectFile)
        {
            if (!projectFile.Exists)
            {
                throw new ArgumentException("Project file does not exist");
            }


            var newFileName = projectFile.Name.Replace(projectFile.Extension, ".json");
            var jsonFilePath = Path.Combine(projectFile.DirectoryName, newFileName);
            var jsonFile = new FileInfo(jsonFilePath);

            if (jsonFile.Exists)
            {
                jsonFile.Delete();
            }

            TextWriter textWriter = new StreamWriter(jsonFile.FullName);
            _jsonWriter = new JsonTextWriter(textWriter);

            _jsonWriter.WriteStartObject();
            _jsonWriter.WritePropertyName("File");
            _jsonWriter.WriteValue(projectFile.FullName);
            _jsonWriter.WritePropertyName("Date");
            _jsonWriter.WriteValue(DateTime.Now);
            _jsonWriter.WritePropertyName("Diagnostics");
            _jsonWriter.WriteStartArray();


            _jsonSerializer.Formatting = Formatting.Indented;
        }
    }
}