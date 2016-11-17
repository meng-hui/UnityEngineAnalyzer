using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

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

        private Dictionary<ExpressionSyntax,ExpressionSyntax> _indirectCallers;


        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.DoNotUseFindMethodsInUpdate, 
                    DiagnosticDescriptors.DoNotUseFindMethodsInUpdateRecursive);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeClassSyntax, SyntaxKind.ClassDeclaration);
        }

        public void AnalyzeClassSyntax(SyntaxNodeAnalysisContext context)
        {
            var monoBehaviourInfo = new MonoBehaviourInfo(context);

            var searched = new Dictionary<IMethodSymbol, bool>();
            _indirectCallers = new Dictionary<ExpressionSyntax, ExpressionSyntax>();

            monoBehaviourInfo.ForEachUpdateMethod((updateMethod) =>
            {
                //Debug.WriteLine("Found an update call! " + updateMethod);

                var findCalls = SearchForFindCalls(context, updateMethod, searched, true);

                foreach (var findCall in findCalls)
                {
                    if (!_indirectCallers.ContainsKey(findCall))
                    {
                        var diagnostic = Diagnostic.Create(DiagnosticDescriptors.DoNotUseFindMethodsInUpdate,
                            findCall.GetLocation(), findCall, monoBehaviourInfo.ClassName, updateMethod.Identifier);
                        context.ReportDiagnostic(diagnostic);
                    }
                    else
                    {
                        var endPoint = _indirectCallers[findCall];

                        var diagnostic = Diagnostic.Create(DiagnosticDescriptors.DoNotUseFindMethodsInUpdateRecursive,
                            findCall.GetLocation(), monoBehaviourInfo.ClassName, updateMethod.Identifier, findCall, endPoint);
                        context.ReportDiagnostic(diagnostic);
                    }

                }
            });
        }

        //TODO: Try to simplify this method - it's very hard to follow
        private IEnumerable<ExpressionSyntax> SearchForFindCalls(SyntaxNodeAnalysisContext context,
            MethodDeclarationSyntax method, IDictionary<IMethodSymbol, bool> searched, bool isRoot)
        {
            var invocations = method.DescendantNodes().OfType<InvocationExpressionSyntax>();

            foreach (var invocation in invocations)
            {
                SymbolInfo symbolInfo;
                if (!context.TryGetSymbolInfo(invocation, out symbolInfo))
                {
                    continue;
                }

                var methodSymbol = symbolInfo.Symbol as IMethodSymbol;

                if (methodSymbol != null)
                {
                    if (searched.ContainsKey(methodSymbol))
                    {
                        if (searched[methodSymbol])
                        {
                            yield return invocation;
                        }
                    }
                    else
                    {
                        if (FindMethodNames.Contains(methodSymbol.Name) &&
                            ContainingSymbols.Contains(methodSymbol.ContainingSymbol.ToString()))
                        {
                            searched.Add(methodSymbol, true);
                            yield return invocation;
                        }
                        else
                        {
                            var methodDeclarations = methodSymbol.DeclaringSyntaxReferences;

                            searched.Add(methodSymbol, false); //let's assume there won't be any calls

                            foreach (var methodDeclaration in methodDeclarations)
                            {
                                var theMethodSyntax = methodDeclaration.GetSyntax() as MethodDeclarationSyntax;

                                if (theMethodSyntax != null)
                                {
                                    var childFindCallers = SearchForFindCalls(context, theMethodSyntax, searched, false);

                                    if (childFindCallers != null && childFindCallers.Any())
                                    {
                                        searched[methodSymbol] = true; //update the searched dictionary with new info

                                        if (isRoot)
                                        {
                                            _indirectCallers.Add(invocation, childFindCallers.First());
                                        }

                                        yield return invocation;
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
}
