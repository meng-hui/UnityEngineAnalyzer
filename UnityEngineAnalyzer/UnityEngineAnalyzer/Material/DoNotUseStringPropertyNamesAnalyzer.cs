using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace UnityEngineAnalyzer.Material
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DoNotUseStringPropertyNamesAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DiagnosticDescriptors.DoNotUseStringPropertyNames);

        private static readonly ImmutableHashSet<string> materialStringPropertyMethods = ImmutableHashSet.Create(
        "GetColor",
        "GetColorArray",
        "GetFloat",
        "GetFloatArray",
        "GetInt",
        "GetMatrix",
        "GetMatrixArray",
        "GetTexture",
        "GetTextureOffset",
        "GetTextureScale",
        "GetVector",
        "GetVectorArray",
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
        "SetVectorArray");

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
            if (!materialStringPropertyMethods.Contains(name)) { return; }

            var symbolInfo = context.SemanticModel.GetSymbolInfo(invocation);
            var methodSymbol = symbolInfo.Symbol as IMethodSymbol;

            var containingClass = methodSymbol.ContainingType;

            // check if the method is the one from UnityEngine.Material
            if (containingClass.ContainingNamespace.Name.Equals("UnityEngine") && containingClass.Name.Equals("Material"))
            {
                if (methodSymbol.Parameters[0].Type.MetadataName == "String")
                {
                    var diagnostic = Diagnostic.Create(DiagnosticDescriptors.DoNotUseStringPropertyNames, invocation.GetLocation(), containingClass.Name, methodSymbol.Name);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}
