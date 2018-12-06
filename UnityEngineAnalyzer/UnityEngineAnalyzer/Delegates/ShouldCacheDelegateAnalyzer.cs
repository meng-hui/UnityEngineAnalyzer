using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace UnityEngineAnalyzer.Delegates
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class ShouldCacheDelegateAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DiagnosticDescriptors.ShouldCacheDelegate);
        
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeClassSyntax, SyntaxKind.ClassDeclaration);
        }

        public static void AnalyzeClassSyntax(SyntaxNodeAnalysisContext context)
        {
            var monoBehaviourInfo = new MonoBehaviourInfo(context);
            
            var searched = new Dictionary<IMethodSymbol, bool>();
            monoBehaviourInfo.ForEachUpdateMethod((updateMethod) =>
            {
                AnalyzeAssignmentNode(context, updateMethod);
                AnalyzeInvocationNode(context, updateMethod);
            });
        }

        public static event Action<int> e;
        private static void AnalyzeAssignmentNode(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax method)
        {
            e += ShouldCacheDelegateAnalyzer_e;
            e -= ShouldCacheDelegateAnalyzer_e;
            foreach (var oneAssign in method.DescendantNodes().OfType<AssignmentExpressionSyntax>())
            {
                AnalyzeAddRemoveNode(context, oneAssign);
            }
        }

        private static void ShouldCacheDelegateAnalyzer_e(int obj)
        {
            throw new NotImplementedException();
        }

        private static void AnalyzeAddRemoveNode(SyntaxNodeAnalysisContext context, ExpressionSyntax syntax)
        {
            foreach (var oneIdSyntax in syntax.ChildNodes().OfType<IdentifierNameSyntax>())
            {
                var oneIdSymbol = context.SemanticModel.GetSymbolInfo(oneIdSyntax);
                if (oneIdSymbol.Symbol != null)
                {
                    if (oneIdSymbol.Symbol is IMethodSymbol methodSymbol && !methodSymbol.IsStatic)
                    {
                        var diagnostic = Diagnostic.Create(DiagnosticDescriptors.ShouldCacheDelegate,
                            oneIdSyntax.GetLocation(), oneIdSyntax.ToString());
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
            foreach (var oneAccessSyntax in syntax.ChildNodes().OfType<MemberAccessExpressionSyntax>())
            {
                foreach (var oneIdSyntax in oneAccessSyntax.ChildNodes().OfType<IdentifierNameSyntax>())
                {
                    var oneIdSymbol = context.SemanticModel.GetSymbolInfo(oneIdSyntax);
                    if (oneIdSymbol.Symbol != null)
                    {
                        if (oneIdSymbol.Symbol is IMethodSymbol methodSymbol && !methodSymbol.IsStatic)
                        {
                            var diagnostic = Diagnostic.Create(DiagnosticDescriptors.ShouldCacheDelegate,
                                oneIdSyntax.GetLocation(), oneIdSyntax.ToString());
                            context.ReportDiagnostic(diagnostic);
                        }
                    }
                }
            }
        }


        private static void AnalyzeInvocationNode(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax method)
        {
            foreach(var invoke in method.DescendantNodes().OfType<InvocationExpressionSyntax>())
            {
                var argumentListSyntax = invoke.ChildNodes().OfType<ArgumentListSyntax>();
                foreach (var oneArgListSyntax in argumentListSyntax)
                {
                    foreach (var oneArgSyntax in oneArgListSyntax.ChildNodes().OfType<ArgumentSyntax>())
                    {
                        foreach (var oneIdSyntax in oneArgSyntax.ChildNodes().OfType<IdentifierNameSyntax>())
                        {
                            var oneIdSymbol = context.SemanticModel.GetSymbolInfo(oneIdSyntax);
                            if (oneIdSymbol.Symbol != null)
                            {
                                if (oneIdSymbol.Symbol is IMethodSymbol methodSymbol && !methodSymbol.IsStatic)
                                {
                                    var diagnostic = Diagnostic.Create(DiagnosticDescriptors.ShouldCacheDelegate,
                                        oneIdSyntax.GetLocation(), oneIdSyntax.ToString());
                                    context.ReportDiagnostic(diagnostic);
                                }
                            }
                        }
                    }
                }
            }

        }
    }
}
