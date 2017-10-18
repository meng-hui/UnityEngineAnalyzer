using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngineAnalyzer.CLI.Reporting;

namespace UnityEngineAnalyzer.CLI
{
    public class Program
    {
        private static readonly Dictionary<string, Type> AvailableExporters = new Dictionary<string, Type>();
        private const UnityVersion DEFAULT_UNITY_VERSION = UnityVersion.LATEST;

        static Program()
        {
            AvailableExporters.Add(nameof(JsonAnalyzerExporter), typeof(JsonAnalyzerExporter));
            AvailableExporters.Add(nameof(StandardOutputAnalyzerReporter), typeof(StandardOutputAnalyzerReporter));
            AvailableExporters.Add(nameof(ConsoleAnalyzerExporter), typeof(ConsoleAnalyzerExporter));
        }

        public static void Main(string[] args)
        {
            try
            {
                var options = new Options();
                var isValid = CommandLine.Parser.Default.ParseArgumentsStrict(args, options);

                if (isValid == false || options.ProjectFile == null)
                {
                    return;
                }

                options.Version = DefineUnityVersion(options);

                var startTime = DateTime.Now;

                var fileName = options.ProjectFile;
                var fileInfo = new FileInfo(fileName);

                //NOTE: This could be configurable via the CLI at some point
                var report = new AnalyzerReport();

                if (options.Exporters != null)
                {
                    foreach (var exporter in options.Exporters)
                    {
                        if (AvailableExporters.ContainsKey(exporter))
                        {
                            var exporterInstance = Activator.CreateInstance(AvailableExporters[exporter]);
                            report.AddExporter(exporterInstance as IAnalyzerExporter);
                        }
                    }
                }

                if (report.GetExporterCount() == 0)
                { 
                    //It's generally a good idea to make sure that the Console Exporter is last since it is interactive
                    report.AddExporter(new JsonAnalyzerExporter(options));
                    report.AddExporter(new ConsoleAnalyzerExporter(options));
                }
                
                report.InitializeReport(options);

                var tasks = new List<Task>();
                if (fileInfo.Exists)
                {
                    FileInfo configFileInfo = null;

                    if (options.ConfigurationFile != null)
                    {
                        configFileInfo = new FileInfo(options.ConfigurationFile);
                    }

                    var solutionAnalyzer = new SolutionAnalyzer();
                    var analyzeTask = solutionAnalyzer.LoadAndAnalyzeProjectAsync(fileInfo, configFileInfo, report);
                    tasks.Add(analyzeTask);
                }

                Task.WaitAll(tasks.ToArray());

                var endTime = DateTime.Now;
                var duration = endTime - startTime;

                report.FinalizeReport(duration);

            }
            catch (Exception generalException)
            {
                
                Console.WriteLine("There was an exception running the analysis");
                Console.WriteLine(generalException.ToString());
            }
        }

        //TODO SET TO OWN CLASS
        private static UnityVersion DefineUnityVersion(Options options)
        {
            if (options.Version != UnityVersion.NONE)
            {
                return options.Version;
            }

            //THIS ONLY WORKS ON UNITY >= 5, before that ProjectVersion.txt did not exists
            var projectPath = new FileInfo(options.ProjectFile).Directory;
            var projectVersionFile = new FileInfo(projectPath + "/ProjectSettings/ProjectVersion.txt");
            if (projectVersionFile.Exists)
            {
                var projectVersionString = File.ReadAllText(projectVersionFile.FullName);
                return TryParseUnityVersion(projectVersionString);
            }

            return DEFAULT_UNITY_VERSION;
        }

        //TODO UNIT TESTS
        private static UnityVersion TryParseUnityVersion(string version)
        {
            string editorText = "m_EditorVersion: ";
            var match = Regex.Match(version, editorText + "[0-9.a-z]*");

            string src = match.Value.Substring(editorText.Length);
            src = src.Replace('.', '_');
            src = src.Substring(0, src.IndexOf('_') + 2);

            var unityVersions = Enum.GetValues(typeof(UnityVersion)).Cast<UnityVersion>();
            foreach (var unityVersion in unityVersions)
            {
                if (Enum.GetName(typeof(UnityVersion), unityVersion).Contains(src))
                {
                    return unityVersion;
                }
            }

            return DEFAULT_UNITY_VERSION;
        }
    }
}
