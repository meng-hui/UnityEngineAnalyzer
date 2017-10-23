using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace UnityEngineAnalyzer.Physics
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class UseNonAllocMethodsAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DiagnosticDescriptors.UseNonAllocMethods);

        private static readonly ImmutableHashSet<string> PhysicsAllocatingCasts = ImmutableHashSet.Create(
        "BoxCastAll",
        "CapsuleCastAll",
        "OverlapBox",
        "OverlapCapsule",
        "OverlapSphere",
        "RaycastAll",
        "SphereCastAll");

        private static readonly ImmutableHashSet<string> Physics2DAllocatingCasts = ImmutableHashSet.Create(
        "BoxCastAll",
        "CapsuleCastAll",
        "CircleCastAll",
        "GetRayIntersectionAll",
        "LinecastAll",
        "OverlapAreaAll",
        "OverlapBoxAll",
        "OverlapCapsuleAll",
        "OverlapCircleAll",
        "OverlapPointAll",
        "RaycastAll");

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

            List<string> containingClassName = new List<string>();

            // check if any of the methods are used
            if (PhysicsAllocatingCasts.Contains(name))
            {
                containingClassName.Add("Physics");
            }

            if (Physics2DAllocatingCasts.Contains(name))
            {
                containingClassName.Add("Physics2D");
            }
            
            if (containingClassName.Count == 0)
            {
                return;
            }

            var symbolInfo = context.SemanticModel.GetSymbolInfo(invocation);
            var methodSymbol = symbolInfo.Symbol as IMethodSymbol;

            var containingType = methodSymbol.ContainingType;

            // check if the method is the one from UnityEngine.Physics
            if (containingType.ContainingNamespace.Name.Equals("UnityEngine") && containingClassName.Contains(containingType.Name))
            {
                var diagnostic = Diagnostic.Create(DiagnosticDescriptors.UseNonAllocMethods, invocation.GetLocation(), containingType.Name, methodSymbol.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
