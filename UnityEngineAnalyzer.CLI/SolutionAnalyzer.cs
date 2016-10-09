using System;
using System.Collections.Immutable;
using System.IO;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.MSBuild;

using UnityEngineAnalyzer.FindMethodsInUpdate;

namespace UnityEngineAnalyzer.CLI
{
    public class SolutionAnalyzer
    {
        public void Analyze(FileInfo solutionFile)
        {
            var workspace = MSBuildWorkspace.Create();
            var project = workspace.OpenProjectAsync(solutionFile.FullName).Result;

            var analyzers = ImmutableArray.CreateBuilder<DiagnosticAnalyzer>();
            analyzers.Add( new DoNotUseFindMethodsInUpdateAnalyzer());

            var compilation = project.GetCompilationAsync().Result;

            var diagnosticResults = compilation.WithAnalyzers(analyzers.ToImmutable()).GetAnalyzerDiagnosticsAsync().Result;

            foreach (var diagnosticResult in diagnosticResults)
            {

                Console.WriteLine(diagnosticResult.GetMessage());

            }
        }
    }
}
