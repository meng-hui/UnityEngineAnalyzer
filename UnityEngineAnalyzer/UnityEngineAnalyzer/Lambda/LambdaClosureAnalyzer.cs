using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace UnityEngineAnalyzer.Language
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class LambdaClosureAnalyzer : DiagnosticAnalyzer
    {
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalysisLambda, SyntaxKind.SimpleLambdaExpression, SyntaxKind.ParenthesizedLambdaExpression);
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(DiagnosticDescriptors.LambdaClosure);

        private void AnalysisLambda(SyntaxNodeAnalysisContext context)
        {
            var lambdaNode = (LambdaExpressionSyntax) context.Node;
            var des = context.SemanticModel.GetSymbolInfo(lambdaNode).Symbol;

            var childrenNodes =
                from node in lambdaNode.DescendantNodes().OfType<IdentifierNameSyntax>()
                let symbol = context.SemanticModel.GetSymbolInfo(node).Symbol
                where symbol.Kind == SymbolKind.Local && symbol.ContainingSymbol != des
                select node;

            foreach (var node in childrenNodes)
            {
                var diagnostic =
                    Diagnostic.Create(SupportedDiagnostics.First(), node.GetLocation(), node.Identifier.Text);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
