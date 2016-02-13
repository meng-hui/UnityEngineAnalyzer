using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace UnityEngineAnalyzer.AOT
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DoNotUseReflectionEmitAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DiagnosticDescriptors.DoNotUseReflectionEmit);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.UsingDirective);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            // retrieve using syntax node
            var syntax = context.Node as UsingDirectiveSyntax;
            // and check if it is System.Reflection.Emit
            if (syntax.Name.ToString().Equals("System.Reflection.Emit"))
            {
                var diagnostic = Diagnostic.Create(DiagnosticDescriptors.DoNotUseReflectionEmit, syntax.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
