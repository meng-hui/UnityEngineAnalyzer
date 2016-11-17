using System;
using System.Collections.Immutable;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.MSBuild;
using UnityEngineAnalyzer.ForEachInUpdate;

namespace UnityEngineAnalyzer.CLI
{
    public class SolutionAnalyzer
    {
        public async Task LoadAnadAnalyzeProject(FileInfo projectFile, AnalyzerReport report) //TODO: Add async suffix
        {
            var workspace = MSBuildWorkspace.Create();

            var project = await workspace.OpenProjectAsync(projectFile.FullName, CancellationToken.None);

            var analyzers = this.GetAnalyzers();

            await AnalyzeProject(project, analyzers, report);
        }

        private ImmutableArray<DiagnosticAnalyzer> GetAnalyzers()
        {
            var listBuilder = ImmutableArray.CreateBuilder<DiagnosticAnalyzer>();

            var assembly = typeof(DoNotUseForEachInUpdate).Assembly;
            var allTypes = assembly.DefinedTypes;

            foreach (var type in allTypes)
            {
                if (type.BaseType == typeof(DiagnosticAnalyzer))
                {
                    var instance = Activator.CreateInstance(type) as DiagnosticAnalyzer;
                    listBuilder.Add(instance);
                }
            }


            var analyzers = listBuilder.ToImmutable();
            return analyzers;
        }

        private async Task AnalyzeProject(Project project, ImmutableArray<DiagnosticAnalyzer> analyzers, AnalyzerReport report)
        {
            var compilation = await project.GetCompilationAsync();

            var diagnosticResults = await compilation.WithAnalyzers(analyzers).GetAnalyzerDiagnosticsAsync();

            report.AppendDiagnostics(diagnosticResults);

        }
    }
}
