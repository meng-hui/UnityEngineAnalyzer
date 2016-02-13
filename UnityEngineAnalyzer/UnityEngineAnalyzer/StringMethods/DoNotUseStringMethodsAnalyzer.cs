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
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.IdentifierName);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var identifierName = context.Node as IdentifierNameSyntax;
            if (!StringMethods.Contains(identifierName.Identifier.ToString())) { return; }

            var methodSymbol = context.SemanticModel.GetSymbolInfo(identifierName).Symbol as IMethodSymbol;
            if (Namespaces.Any(ns => methodSymbol?.ToString().StartsWith(ns) ?? false))
            {
                var diagnostic = Diagnostic.Create(DiagnosticDescriptors.DoNotUseStringMethods, identifierName.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
