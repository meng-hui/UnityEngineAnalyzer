using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace UnityEngineAnalyzer.Coroutines
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DoNotUseCoroutinesAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DiagnosticDescriptors.DoNotUseCoroutines);
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            // check if we have a method invocation
            var invocationExpression = context.Node as InvocationExpressionSyntax;
            var methodSymbol = context.SemanticModel.GetSymbolInfo(invocationExpression).Symbol as IMethodSymbol;
            if (methodSymbol == null) { return; }

            // check that the method is StartCoroutine from UnityEngine
            if (methodSymbol.Name.Equals("StartCoroutine") && 
                methodSymbol.ContainingSymbol.ToString().Equals("UnityEngine.MonoBehaviour"))
            {
                var diagnostic = Diagnostic.Create(DiagnosticDescriptors.DoNotUseCoroutines, invocationExpression.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
