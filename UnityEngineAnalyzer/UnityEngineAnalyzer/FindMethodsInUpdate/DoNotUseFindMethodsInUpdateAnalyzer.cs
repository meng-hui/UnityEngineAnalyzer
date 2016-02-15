using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace UnityEngineAnalyzer.FindMethodsInUpdate
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DoNotUseFindMethodsInUpdateAnalyzer : DiagnosticAnalyzer
    {
        private static readonly ImmutableHashSet<string> FindMethodNames = ImmutableHashSet.Create(
            "Find",
            "FindGameObjectsWithTag",
            "FindGameObjectWithTag",
            "FindWithTag",
            "FindObjectOfType",
            "FindObjectsOfType",
            "FindObjectsOfTypeAll",
            "FindObjectsOfTypeIncludingAssets",
            "FindSceneObjectsOfType",
            "GetComponent",
            "GetComponentInChildren",
            "GetComponentInParent",
            "GetComponents",
            "GetComponentsInChildren",
            "GetComponentsInParent",
            "FindChild");

        private static readonly ImmutableHashSet<string> ContainingSymbols = ImmutableHashSet.Create(
            "UnityEngine.GameObject",
            "UnityEngine.Object",
            "UnityEngine.Component",
            "UnityEngine.Transform");

        private static readonly ImmutableHashSet<string> UpdateMethodNames = ImmutableHashSet.Create(
            "OnGUI",
            "Update",
            "FixedUpdate",
            "LateUpdate");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DiagnosticDescriptors.DoNotUseFindMethodsInUpdate);
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            // get the invocation expression
            var invocationExpression = context.Node as InvocationExpressionSyntax;
            // get the method symbol
            var methodSymbol = context.SemanticModel.GetSymbolInfo(invocationExpression).Symbol as IMethodSymbol;
            if (methodSymbol == null) { return; }

            // check if we have found a Find* or Get* method from UnityEngine
            if (!FindMethodNames.Contains(methodSymbol.Name) ||
                !ContainingSymbols.Contains(methodSymbol.ContainingSymbol.ToString())) { return; }

            // retrive the method this invocation happened in
            var outerMethodSyntax = invocationExpression.FirstAncestorOrSelf<MethodDeclarationSyntax>();
            if (outerMethodSyntax == null) { return; }

            // check if we are immediately in a Update* method
            var outerMethodSymbol = context.SemanticModel.GetDeclaredSymbol(outerMethodSyntax);
            if (outerMethodSymbol == null) { return; }
            if (!UpdateMethodNames.Contains(outerMethodSymbol.Name)) { return; }

            // check if the Update* method is from UnityEngine
            var containingClass = outerMethodSymbol.ContainingType;
            var baseClass = containingClass.BaseType;
            if (baseClass.ContainingNamespace.Name.Equals("UnityEngine") &&
                baseClass.Name.Equals("MonoBehaviour"))
            {
                var diagnostic = Diagnostic.Create(DiagnosticDescriptors.DoNotUseFindMethodsInUpdate, invocationExpression.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
