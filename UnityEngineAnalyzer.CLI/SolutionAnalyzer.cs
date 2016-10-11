using System;
using System.Collections.Immutable;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.MSBuild;
using UnityEngineAnalyzer.CompareTag;
using UnityEngineAnalyzer.EmptyMonoBehaviourMethods;
using UnityEngineAnalyzer.FindMethodsInUpdate;
using UnityEngineAnalyzer.ForEachInUpdate;

namespace UnityEngineAnalyzer.CLI
{
    public class SolutionAnalyzer
    {
        public async Task LoadAnadAnalyzeProject(FileInfo projectFile)
        {
            var workspace = MSBuildWorkspace.Create();

            var project = await workspace.OpenProjectAsync(projectFile.FullName, CancellationToken.None);

            var analyzers = this.GetAnalyzers();

            await AnalyzeProject(project, analyzers);
        }

        private ImmutableArray<DiagnosticAnalyzer> GetAnalyzers()
        {
            var listBuilder = ImmutableArray.CreateBuilder<DiagnosticAnalyzer>();
            listBuilder.Add(new DoNotUseFindMethodsInUpdateAnalyzer());
            listBuilder.Add(new EmptyMonoBehaviourMethodsAnalyzer());
            listBuilder.Add(new UseCompareTagAnalyzer());
            listBuilder.Add(new DoNotUseForEachInUpdate());
            //NOTE: We could use Reflection to automatically pick up all of the Analyzers


            var analyzers = listBuilder.ToImmutable();
            return analyzers;
        }

        private async Task AnalyzeProject(Project project, ImmutableArray<DiagnosticAnalyzer> analyzers)
        {
            var compilation = await project.GetCompilationAsync();

            var diagnosticResults = await compilation.WithAnalyzers(analyzers).GetAnalyzerDiagnosticsAsync();

            foreach (var diagnosticResult in diagnosticResults)
            {
                var record = string.Join("\t",diagnosticResult.Id,diagnosticResult.GetMessage(), diagnosticResult.Location);

                Console.WriteLine(record);
            }
        }
    }
}
