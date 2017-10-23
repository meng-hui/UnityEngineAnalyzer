using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace UnityEngineAnalyzer.CLI.Reporting
{
    public class JsonAnalyzerExporter : AnalyzerExporter
    {
        private const string JsonReportFileName = "report.json";
        private const string HtmlReportFileName = "UnityReport.html";

        private JsonTextWriter _jsonWriter;
        private readonly JsonSerializer _jsonSerializer = new JsonSerializer();
        private readonly List<Exception> _exceptions = new List<Exception>();
        private string _destinationReportFile;

        public JsonAnalyzerExporter(Options options) : base(options)
        {
        }

        public override void AppendDiagnostic(DiagnosticInfo diagnosticInfo)
        {
            if (IsAnalyzerRelevant(diagnosticInfo))
            {
                _jsonSerializer.Serialize(_jsonWriter, diagnosticInfo);
            }
        }

        public override void FinalizeExporter(TimeSpan duration)
        {
            _jsonWriter.WriteEndArray();

            _jsonWriter.WritePropertyName("Exceptions");
            _jsonWriter.WriteStartArray();

            foreach (var exception in _exceptions)
            {
                _jsonSerializer.Serialize(_jsonWriter, exception);
            }
            _jsonWriter.WriteEndArray();

            _jsonWriter.WriteEndObject();
            _jsonWriter.Close();

            //Console.WriteLine(Process.GetCurrentProcess().StartInfo.WorkingDirectory);
            File.Copy(HtmlReportFileName, _destinationReportFile, true);

            //NOTE: This code might be temporary as it assumes that the CLI is being executed interactively
            //Process.Start(_destinationReportFile);
        }

        public override void InitializeExporter(Options options)
        {
            var projectFile = new FileInfo(options.ProjectFile);
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

        public override void NotifyException(Exception exception)
        {
            _exceptions.Add(exception);
        }
    }
}