using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace UnityEngineAnalyzer.StringMethods
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DoNotUseStringMethodsAnalyzer : DiagnosticAnalyzer
    {
        private static readonly ImmutableHashSet<string> StringMethods = ImmutableHashSet.Create("SendMessage", "SendMessageUpwards", "BroadcastMessage", "Invoke", "InvokeRepeating");
        private static readonly ImmutableHashSet<string> Namespaces = ImmutableHashSet.Create("UnityEngine.Component", "UnityEngine.GameObject", "UnityEngine.MonoBehaviour");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DiagnosticDescriptors.DoNotUseStringMethods);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var invocation = context.Node as InvocationExpressionSyntax;
            if (invocation == null)
            {
                return;
            }


            string name = null;
            if (invocation.Expression is MemberAccessExpressionSyntax)
            {
                name = ((MemberAccessExpressionSyntax)invocation.Expression).Name.Identifier.ToString();
            }
            else if (invocation.Expression is IdentifierNameSyntax)
            {
                name = ((IdentifierNameSyntax)invocation.Expression).ToString();
            }
            else if (invocation.Expression is GenericNameSyntax)
            {
                name = ((GenericNameSyntax)invocation.Expression).Identifier.ToString();
            }


            // check if any of the "string" methods are used
            if (!StringMethods.Contains(name)) { return; }


            // check if the method is the one from UnityEngine
            var symbolInfo = context.SemanticModel.GetSymbolInfo(invocation);
            var methodSymbol = symbolInfo.Symbol as IMethodSymbol;

            if (Namespaces.Any(ns => methodSymbol?.ToString().StartsWith(ns) ?? false))
            {
                var diagnostic = Diagnostic.Create(DiagnosticDescriptors.DoNotUseStringMethods, invocation.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
