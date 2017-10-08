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
    public sealed class DoNotUseCameraMainInUpdateAnalyzer : DiagnosticAnalyzer
    {

        private Dictionary<ExpressionSyntax,ExpressionSyntax> _indirectCallers;


        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.DoNotUseCameraMainInUpdate,
                    DiagnosticDescriptors.DoNotUseCameraMainInUpdateRecursive);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeClassSyntax, SyntaxKind.ClassDeclaration);
        }

        public void AnalyzeClassSyntax(SyntaxNodeAnalysisContext context)
        {
            var monoBehaviourInfo = new MonoBehaviourInfo(context);

            var searched = new Dictionary<ISymbol, bool>();
            _indirectCallers = new Dictionary<ExpressionSyntax, ExpressionSyntax>();

            monoBehaviourInfo.ForEachUpdateMethod((updateMethod) =>
            {
                //Debug.WriteLine("Found an update call! " + updateMethod);

                var results = SearchForCameraMain(context, updateMethod, searched, true);

                foreach (var oneResult in results)
                {
                    if (!_indirectCallers.ContainsKey(oneResult))
                    {
                        var diagnostic = Diagnostic.Create(DiagnosticDescriptors.DoNotUseCameraMainInUpdate,
                            oneResult.GetLocation(), oneResult, monoBehaviourInfo.ClassName, updateMethod.Identifier);
                        context.ReportDiagnostic(diagnostic);
                    }
                    else
                    {
                        var endPoint = _indirectCallers[oneResult];

                        var diagnostic = Diagnostic.Create(DiagnosticDescriptors.DoNotUseCameraMainInUpdateRecursive,
                            oneResult.GetLocation(), monoBehaviourInfo.ClassName, updateMethod.Identifier, oneResult, endPoint);
                        context.ReportDiagnostic(diagnostic);
                    }

                }
            });
        }

        //TODO: Try to simplify this method - it's very hard to follow
        private IEnumerable<ExpressionSyntax> SearchForCameraMain(SyntaxNodeAnalysisContext context,
            MethodDeclarationSyntax method, IDictionary<ISymbol, bool> searchedSymbol, bool isRoot)
        {
            var accessExps = method.DescendantNodes().OfType<MemberAccessExpressionSyntax>();
            foreach (var oneAccessExp in accessExps)
            {
                SymbolInfo oneSymbolInfo;
                if (!context.TryGetSymbolInfo(oneAccessExp, out oneSymbolInfo))
                {
                    continue;
                }

                var propertySymbol = oneSymbolInfo.Symbol as IPropertySymbol;
                if (propertySymbol != null)
                {
                    if (searchedSymbol.ContainsKey(propertySymbol))
                    {
                        if (searchedSymbol[propertySymbol])
                        {
                            yield return (ExpressionSyntax)oneAccessExp;
                        }
                    }
                    else
                    {
                        if (propertySymbol.Name == "main" &&
                            propertySymbol.ContainingSymbol.ToString() == "UnityEngine.Camera")
                        {
                            searchedSymbol.Add(propertySymbol, true);
                            yield return oneAccessExp;
                        }
                    }
                }
            }


            var invocationExps = method.DescendantNodes().OfType<InvocationExpressionSyntax>();
            foreach (var oneInvocationExp in invocationExps)
            {
                SymbolInfo oneSymbolInfo;
                if (!context.TryGetSymbolInfo(oneInvocationExp, out oneSymbolInfo))
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
                                var childResults = SearchForCameraMain(context, theMethodSyntax, searchedSymbol, false);

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
