using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace UnityEngineAnalyzer.LogicError
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    /// InfiniteRecursiveCallAnalyzer currently is conservative,
    /// which means, if the recursive call is inside a branch (if/switch),
    /// then this analyzer will NOT report diagnose.
    public sealed class InfiniteRecursiveCallAnalyzer : DiagnosticAnalyzer
    {

        private Dictionary<ExpressionSyntax, ExpressionSyntax> _directCallers;
        private Dictionary<ExpressionSyntax,ExpressionSyntax> _indirectCallers;


        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.InfiniteRecursiveCall,
                    DiagnosticDescriptors.InfiniteRecursiveCallRecursive);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.ClassDeclaration);
        }

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var searched = new Dictionary<ISymbol, bool>();
            _directCallers = new Dictionary<ExpressionSyntax, ExpressionSyntax>();
            _indirectCallers = new Dictionary<ExpressionSyntax, ExpressionSyntax>();

            List<ExpressionSyntax> reported = new List<ExpressionSyntax>();

            foreach (var oneMethodDeclaration in context.Node.ChildNodes().OfType<MethodDeclarationSyntax>())
            {
                var methodSymbol = context.SemanticModel.GetDeclaredSymbol(oneMethodDeclaration) as IMethodSymbol;


                var results = SearchForTargetExpression(context, methodSymbol, oneMethodDeclaration, searched, true);

                foreach (var oneResult in results)
                {
                    if (!reported.Contains((oneResult)))
                    {
                        if (_directCallers.ContainsKey(oneResult))
                        {
                            var diagnostic = Diagnostic.Create(DiagnosticDescriptors.InfiniteRecursiveCall,
                                oneResult.GetLocation(), methodSymbol.ContainingType.Name, oneMethodDeclaration.Identifier);
                            context.ReportDiagnostic(diagnostic);
                            reported.Add(oneResult);

                        }
                        else if (_indirectCallers.ContainsKey(oneResult))
                        {
                            var endPoint = _indirectCallers[oneResult];

                            var diagnostic = Diagnostic.Create(DiagnosticDescriptors.InfiniteRecursiveCallRecursive,
                                oneResult.GetLocation(), methodSymbol.ContainingType.Name, oneMethodDeclaration.Identifier, oneResult);
                            context.ReportDiagnostic(diagnostic);
                            reported.Add(oneResult);
                        }
                    }

                }
            }

        }
        

        //TODO: Try to simplify this currMethodSyntax - it's very hard to follow
        private IEnumerable<ExpressionSyntax> SearchForTargetExpression(SyntaxNodeAnalysisContext context,
            IMethodSymbol checkMethodSymbol,
            MethodDeclarationSyntax currMethodSyntax, IDictionary<ISymbol, bool> searchedSymbol, bool isRoot)
        {
            foreach (var oneTargetExp in currMethodSyntax.DescendantNodes().OfType<InvocationExpressionSyntax>())
            {
                /*
                var oneTargetIdentifierExps = oneTargetExp.DescendantNodes().OfType<IdentifierNameSyntax>();
                foreach(var oneTargetIdentifierExp in oneTargetIdentifierExps)
                {
                }
                */

                bool isContainedByBranch = false;

                SyntaxNode parent = oneTargetExp;
                while (parent != currMethodSyntax)
                {
                    if (parent is IfStatementSyntax || parent is SwitchStatementSyntax || parent is ForStatementSyntax ||
                        parent is ForEachStatementSyntax || parent is WhileStatementSyntax)
                    {
                        isContainedByBranch = true;
                        break;
                    }

                    parent = parent.Parent;
                }

                if (isContainedByBranch)
                {
                    continue;
                }

                bool hasPrecedingReturn = false;

                foreach (var oneReturnSyntax in currMethodSyntax.DescendantNodes().OfType<ReturnStatementSyntax>())
                {
                    if (oneReturnSyntax.SpanStart < oneTargetExp.SpanStart)
                    {
                        hasPrecedingReturn = true;
                        break;
                    }
                }
                if (hasPrecedingReturn)
                {
                    continue;
                }

                SymbolInfo oneSymbolInfo;
                if (!context.TryGetSymbolInfo(oneTargetExp, out oneSymbolInfo))
                {
                    continue;
                }

                var targetSymbol = oneSymbolInfo.Symbol as IMethodSymbol;
                if (targetSymbol != null)
                {
                    if (searchedSymbol.ContainsKey(targetSymbol))
                    {
                        if (searchedSymbol[targetSymbol])
                        {
                            yield return (ExpressionSyntax)oneTargetExp;
                        }
                    }
                    else
                    {
                        if (targetSymbol == checkMethodSymbol)
                        {
                            searchedSymbol.Add(targetSymbol, true);
                            if (isRoot)
                            {
                                _directCallers.Add(oneTargetExp, oneTargetExp);
                            }
                            yield return oneTargetExp;
                        }
                    }
                }
            }


            var invocationExps = currMethodSyntax.DescendantNodes().OfType<InvocationExpressionSyntax>();
            foreach (var oneInvocationExp in invocationExps)
            {
                SymbolInfo oneSymbolInfo;
                if (!context.TryGetSymbolInfo(oneInvocationExp, out oneSymbolInfo))
                {
                    continue;
                }


                bool isContainedByBranch = false;
                SyntaxNode parent = oneInvocationExp;
                while (parent != currMethodSyntax)
                {
                    if (parent is IfStatementSyntax || parent is SwitchStatementSyntax)
                    {
                        isContainedByBranch = true;
                        break;
                    }

                    parent = parent.Parent;
                }
                if (isContainedByBranch)
                {
                    continue;
                }

                var methodSymbol = oneSymbolInfo.Symbol as IMethodSymbol;

                if (methodSymbol != null)
                {
                    if (searchedSymbol.ContainsKey(methodSymbol))
                    {
                        if (searchedSymbol[methodSymbol])
                        {
                            yield return oneInvocationExp;
                        }
                    }
                    else
                    {
                        var methodDeclarations = methodSymbol.DeclaringSyntaxReferences;
                        searchedSymbol.Add(methodSymbol, false); //let's assume there won't be any calls

                        foreach (var methodDeclaration in methodDeclarations)
                        {
                            var theMethodSyntax = methodDeclaration.GetSyntax() as MethodDeclarationSyntax;

                            if (theMethodSyntax != null)
                            {
                                var childResults = SearchForTargetExpression(context, checkMethodSymbol, theMethodSyntax, searchedSymbol, false);

                                if (childResults != null && childResults.Any())
                                {
                                    searchedSymbol[methodSymbol] = true; //update the searched dictionary with new info

                                    if (isRoot)
                                    {
                                        _indirectCallers.Add(oneInvocationExp, childResults.First());
                                    }

                                    yield return oneInvocationExp;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
