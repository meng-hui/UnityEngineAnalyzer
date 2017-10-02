using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
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

            string containingClassName = string.Empty;
            // check if any of the methods are used
            if (PhysicsAllocatingCasts.Contains(name))
            {
                containingClassName = "Physics";
            }
            else if (Physics2DAllocatingCasts.Contains(name))
            {
                containingClassName = "Physics2D";
            }
            else
            {
                return;
            }

            var symbolInfo = context.SemanticModel.GetSymbolInfo(invocation);
            var methodSymbol = symbolInfo.Symbol as IMethodSymbol;

            var containingType = methodSymbol.ContainingType;

            // check if the method is the one from UnityEngine.Animator
            if (containingType.ContainingNamespace.Name.Equals("UnityEngine") && containingType.Name.Equals(containingClassName))
            {
                var diagnostic = Diagnostic.Create(DiagnosticDescriptors.UseNonAllocMethods, invocation.GetLocation(), containingType.Name, methodSymbol.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
