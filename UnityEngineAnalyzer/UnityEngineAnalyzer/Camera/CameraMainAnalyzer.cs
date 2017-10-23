using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace UnityEngineAnalyzer.Camera
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class CameraMainAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DiagnosticDescriptors.CameraMainIsSlow);
        private List<MemberAccessExpressionSyntax> searched = new List<MemberAccessExpressionSyntax>();

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeClassSyntax, SyntaxKind.ClassDeclaration);
        }

        public void AnalyzeClassSyntax(SyntaxNodeAnalysisContext context)
        {
            var monoBehaviourInfo = new MonoBehaviourInfo(context);

            monoBehaviourInfo.ForEachUpdateMethod((updateMethod) =>
            {
                SearchCameraMain(context, updateMethod);
                RecursiveMethodCrawler(context, updateMethod);
            });
        }

        private void SearchCameraMain(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax method)
        {
            var memberAccessExpression = method.DescendantNodes().OfType<MemberAccessExpressionSyntax>();

            foreach (var memberAccess in memberAccessExpression)
            {
                if (searched.Contains(memberAccess))
                {
                    return;
                }

                searched.Add(memberAccess);

                SymbolInfo symbolInfo;
                if (!context.TryGetSymbolInfo(memberAccess.Expression, out symbolInfo))
                {
                    continue;
                }

                var containingClass = symbolInfo.Symbol.ContainingType;

                if (containingClass != null && containingClass.ContainingNamespace.Name.Equals("UnityEngine") && containingClass.Name.Equals("Camera"))
                {
                    if (symbolInfo.Symbol.Name.Equals("main"))
                    {
                        var diagnostic = Diagnostic.Create(DiagnosticDescriptors.CameraMainIsSlow, memberAccess.GetLocation(), memberAccess.Name, symbolInfo.Symbol.Name);
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }

        private object lastMethodRecursion = null;

        private void RecursiveMethodCrawler(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax method)
        {
            var invocationAccessExpression = method.DescendantNodes().OfType<InvocationExpressionSyntax>();
            SearchCameraMain(context, method);

            foreach (var invocation in invocationAccessExpression)
            {
                if (lastMethodRecursion == invocationAccessExpression)
                {
                    break;
                }

                lastMethodRecursion = invocationAccessExpression;

                SymbolInfo symbolInfo;
                if (!context.TryGetSymbolInfo(invocation, out symbolInfo))
                {
                    continue;
                }

                var methodSymbol = symbolInfo.Symbol as IMethodSymbol;
                var methodDeclarations = methodSymbol.DeclaringSyntaxReferences;

                foreach (var methodDeclaration in methodDeclarations)
                {
                    var theMethodSyntax = methodDeclaration.GetSyntax() as MethodDeclarationSyntax;
                    RecursiveMethodCrawler(context, theMethodSyntax);
                }
            }
        }
    }
}
