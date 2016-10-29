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


        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(DiagnosticDescriptors.DoNotUseFindMethodsInUpdate);

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
                //Debug.WriteLine("Found an update call! " + updateMethod);

                var findCalls = SearchForFindCalls(context, updateMethod, searched);

                foreach (var findCall in findCalls)
                {
                    //Debug.WriteLine("Found a bad call! " + findCall);

                    var diagnostic = Diagnostic.Create(DiagnosticDescriptors.DoNotUseFindMethodsInUpdate,
                        findCall.GetLocation(), findCall, monoBehaviourInfo.ClassName, updateMethod.Identifier);
                    context.ReportDiagnostic(diagnostic);
                }
            });
        }

        //TODO: Try to simplify this method - it's very hard to follow
        private static IEnumerable<ExpressionSyntax> SearchForFindCalls(SyntaxNodeAnalysisContext context,
            MethodDeclarationSyntax method, IDictionary<IMethodSymbol, bool> searched)
        {
            var invocations = method.DescendantNodes().OfType<InvocationExpressionSyntax>();

            foreach (var invocation in invocations)
            {
                SymbolInfo symbolInfo;
                if (!TryGetSymbolInfo(context, invocation, out symbolInfo))
                {
                    continue;
                }
                //var symbolInfo = context.SemanticModel.GetSymbolInfo(invocation);

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
                                    var childFindCallers = SearchForFindCalls(context, theMethodSyntax, searched);

                                    if (childFindCallers != null && childFindCallers.Any())
                                    {
                                        searched[methodSymbol] = true; //update the searched dictionary with new info
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

        private static bool TryGetSymbolInfo(SyntaxNodeAnalysisContext context, SyntaxNode node, out SymbolInfo symbolInfo)
        {
            try
            {
                //NOTE: The Call below fixes many issues where the symbol cannot be found - but there are still cases where an argumentexception is thrown
                // which seems to resemble this issue: https://github.com/dotnet/roslyn/issues/11193

                var semanticModel = SemanticModelFor(context.SemanticModel, node);

                symbolInfo = semanticModel.GetSymbolInfo(node); //context.SemanticModel.GetSymbolInfo(node);
                return true;
            }
            catch (Exception generalException)
            {
                Debug.WriteLine("Unable to find Symbol: " + node);
                Debug.WriteLine(generalException);
            }

            symbolInfo = default(SymbolInfo);
            return false;
        }

        internal static SemanticModel SemanticModelFor(SemanticModel semanticModel, SyntaxNode expression)
        {
            if (ReferenceEquals(semanticModel.SyntaxTree, expression.SyntaxTree))
            {
                return semanticModel;
            }

            //NOTE: there may be a performance boost if we cache some of the semantic models
            return semanticModel.Compilation.GetSemanticModel(expression.SyntaxTree);
        }
    }
}
