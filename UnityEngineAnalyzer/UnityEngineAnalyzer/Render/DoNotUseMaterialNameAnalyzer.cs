using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace UnityEngineAnalyzer.Render
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DoNotUseMaterialNameAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DiagnosticDescriptors.DoNotUseMaterialName);

        private static readonly ImmutableHashSet<string> checkMethods = ImmutableHashSet.Create(
        "GetColor",
        "GetColorArray",
        "GetFloat",
        "GetFloatArray",
        "GetInt",
        "GetMatrix",
        "GetMatrixArray",
        "GetTexture",
        "GetVector",
        "HasProperty",
        "SetBuffer",
        "SetColor",
        "SetColorArray",
        "SetFloat",
        "SetFloatArray",
        "SetInt",
        "SetMatrix",
        "SetMatrixArray",
        "SetTexture",
        "SetTextureOffset",
        "SetTextureScale",
        "SetVector",
        "SetVectorArray"
        );

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

            var name = invocation.MethodName();

            // check if any of the methods are used
            if (!checkMethods.Contains(name)) { return; }

            var symbolInfo = context.SemanticModel.GetSymbolInfo(invocation);
            var methodSymbol = symbolInfo.Symbol as IMethodSymbol;

            var containingClass = methodSymbol.ContainingType;

            // check if the method is the one from UnityEngine.Animator
            if (containingClass.ContainingNamespace.Name.Equals("UnityEngine") && containingClass.Name.Equals("Material"))
            {
                if (methodSymbol.Parameters[0].Type.MetadataName == "String")
                {
                    var diagnostic = Diagnostic.Create(DiagnosticDescriptors.DoNotUseMaterialName, invocation.GetLocation(), containingClass.Name, methodSymbol.Name);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}
