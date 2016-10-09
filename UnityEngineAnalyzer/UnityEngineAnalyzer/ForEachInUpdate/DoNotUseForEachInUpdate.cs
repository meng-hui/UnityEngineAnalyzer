using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace UnityEngineAnalyzer.ForEachInUpdate
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DoNotUseForEachInUpdate : DiagnosticAnalyzer
    {
        private static readonly ImmutableHashSet<string> ContainingSymbols = ImmutableHashSet.Create(
            "UnityEngine.GameObject",
            "UnityEngine.Object",
            "UnityEngine.Component",
            "UnityEngine.Transform");

        private static readonly ImmutableHashSet<string> UpdateMethodNames = ImmutableHashSet.Create(
            "OnGUI",
            "Update",
            "FixedUpdate",
            "LateUpdate");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DiagnosticDescriptors.DoNotUseForEachInUpdate);
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeClassSyntax, SyntaxKind.ClassDeclaration);
        }


        //TODO: This function has a lot in common with the DoNotUseFindInUpdate -- Refactor

        public static void AnalyzeClassSyntax(SyntaxNodeAnalysisContext context)
        {
            var classDeclaration = context.Node as ClassDeclarationSyntax;

            if (classDeclaration != null)
            {
                var classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration);

                if (IsMonoBehavior(classSymbol))
                {
                    var methods = classDeclaration.Members.OfType<MethodDeclarationSyntax>();

                    foreach (var method in methods)
                    {
                        if (UpdateMethodNames.Contains(method.Identifier.ValueText))
                        {
                            var searched = new Dictionary<IMethodSymbol, bool>();

                            var forEachStatements = SearchForForEach(context, method, searched);

                            foreach (var forEachStatement in forEachStatements)
                            {
                                Debug.WriteLine("Found a bad call! " + forEachStatement);

                                var diagnostic = Diagnostic.Create(DiagnosticDescriptors.DoNotUseForEachInUpdate, forEachStatement.GetLocation(), method.Identifier);
                                context.ReportDiagnostic(diagnostic);
                            }
                        }
                    }
                }
            }
        }


        private static IEnumerable<StatementSyntax> SearchForForEach(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax method, IDictionary<IMethodSymbol, bool> searched)
        {
            var statements = new List<StatementSyntax>();

            var invocations = method.DescendantNodes().OfType<ForEachStatementSyntax>();

            foreach (var invocation in invocations)
            {
                //var methodSymbol = context.SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;

               statements.Add(invocation);
            }

            return statements;
        }


        private static bool IsMonoBehavior(INamedTypeSymbol classDeclaration)
        {

            if (classDeclaration.BaseType == null)
            {
                return false;
            }

            var baseClass = classDeclaration.BaseType;

            //TODO: Might have to go up the base class chain
            if (baseClass.ContainingNamespace.Name.Equals("UnityEngine") && baseClass.Name.Equals("MonoBehaviour"))
            {
                return true;
            }

            return IsMonoBehavior(baseClass); //determine if the BaseClass extends mono behavior

        }

    }
}
