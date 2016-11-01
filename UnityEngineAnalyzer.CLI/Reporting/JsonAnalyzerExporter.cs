using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

namespace UnityEngineAnalyzer.CLI.Reporting
{
    public class JsonAnalyzerExporter : IAnalyzerExporter
    {
        private const string JsonReportFileName = "report.json";
        private const string HtmlReportFileName = "UnityReport.html";
        private const DiagnosticInfoSeverity MinimalSeverity = DiagnosticInfoSeverity.Warning;


        private JsonTextWriter _jsonWriter;
        private readonly JsonSerializer _jsonSerializer = new JsonSerializer();
        private string _destinationReportFile;


        public void AppendDiagnostic(DiagnosticInfo diagnosticInfo)
        {
            if (diagnosticInfo.Severity >= MinimalSeverity)
            {
                _jsonSerializer.Serialize(_jsonWriter, diagnosticInfo);
            }
        }

        public void Finish(TimeSpan duration)
        {
            _jsonWriter.WriteEndArray();
            _jsonWriter.WriteEndObject();
            _jsonWriter.Close();

            
            File.Copy(HtmlReportFileName, _destinationReportFile, true);

            //NOTE: This code might be temporary as it assumes that the CLI is being executed interactively
            //Process.Start(_destinationReportFile);
        }

        public void InitializeExporter(FileInfo projectFile)
        {
            if (!projectFile.Exists)
            {
                throw new ArgumentException("Project file does not exist");
            }

            _destinationReportFile = Path.Combine(projectFile.DirectoryName, HtmlReportFileName);
            var jsonFilePath = Path.Combine(projectFile.DirectoryName, JsonReportFileName);
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