using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace UnityEngineAnalyzer.Animator
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DoNotUseStateNameAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DiagnosticDescriptors.DoNotUseStateName);

        private static readonly ImmutableHashSet<string> animatorStateNameMethods = ImmutableHashSet.Create(
        "GetBool",
        "GetFloat",
        "GetInteger",
        "GetVector",
        "GetQuaternion",
        "SetBool",
        "SetFloat",
        "SetInteger",
        "SetVector",
        "SetQuaternion",
        "SetTrigger",
        "PlayInFixedTime",
        "Play",
        "IsParameterControlledByCurve",
        "CrossFade",
        "CrossFadeInFixedTime");

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
            if (!animatorStateNameMethods.Contains(name)) { return; }

            var symbolInfo = context.SemanticModel.GetSymbolInfo(invocation);
            var methodSymbol = symbolInfo.Symbol as IMethodSymbol;

            var containingClass = methodSymbol.ContainingType;

            // check if the method is the one from UnityEngine.Animator
            if (containingClass.ContainingNamespace.Name.Equals("UnityEngine") && containingClass.Name.Equals("Animator"))
            {
                if (methodSymbol.Parameters[0].Type.MetadataName == "String")
                {
                    var diagnostic = Diagnostic.Create(DiagnosticDescriptors.DoNotUseStateName, invocation.GetLocation(), containingClass.Name, methodSymbol.Name);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}
