using System;
using System.Collections.Immutable;
using System.IO;
using System.Threading;
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
        public void LoadAnadAnalyzeProject(FileInfo projectFile)
        {
            //TODO: use async!

            var workspace = MSBuildWorkspace.Create();

            var project = workspace.OpenProjectAsync(projectFile.FullName, CancellationToken.None).Result;

            var analyzers = this.GetAnalyzers();

            AnalyzeProject(project, analyzers);
        }

        private ImmutableArray<DiagnosticAnalyzer> GetAnalyzers()
        {
            var listBuilder = ImmutableArray.CreateBuilder<DiagnosticAnalyzer>();
            listBuilder.Add(new DoNotUseFindMethodsInUpdateAnalyzer());
            listBuilder.Add(new EmptyMonoBehaviourMethodsAnalyzer());
            listBuilder.Add(new UseCompareTagAnalyzer());
            listBuilder.Add(new DoNotUseForEachInUpdate());

            var analyzers = listBuilder.ToImmutable();
            return analyzers;
        }

        private void AnalyzeProject(Project project, ImmutableArray<DiagnosticAnalyzer> analyzers)
        {
            var compilation = project.GetCompilationAsync().Result;

            var diagnosticResults = compilation.WithAnalyzers(analyzers).GetAnalyzerDiagnosticsAsync().Result;

            foreach (var diagnosticResult in diagnosticResults)
            {
                var record = string.Join("\t",diagnosticResult.Id,diagnosticResult.GetMessage(), diagnosticResult.Location);

                Console.WriteLine(record);
            }
        }
    }
}
