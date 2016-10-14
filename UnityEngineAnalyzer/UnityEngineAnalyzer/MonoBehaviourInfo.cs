using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace UnityEngineAnalyzer
{
    class MonoBehaviourInfo
    {

        private readonly INamedTypeSymbol _classSymbol;
        private readonly ClassDeclarationSyntax _classDeclaration;
        
        private static readonly ImmutableHashSet<string> UpdateMethodNames = ImmutableHashSet.Create(
            "OnGUI",
            "Update",
            "FixedUpdate",
            "LateUpdate");

        public MonoBehaviourInfo(SyntaxNodeAnalysisContext analysisContext)
        {
            _classDeclaration = analysisContext.Node as ClassDeclarationSyntax;
            _classSymbol = analysisContext.SemanticModel.GetDeclaredSymbol(_classDeclaration) as INamedTypeSymbol;

            if (_classSymbol != null)
            {
                this.ClassName = _classSymbol.Name;
            }
        }

        public string ClassName { get; private set; }

        public void ForEachUpdateMethod(Action<MethodDeclarationSyntax> callback)
        {
            if (this.IsMonoBehaviour())
            {
                var methods = _classDeclaration.Members.OfType<MethodDeclarationSyntax>();

                foreach (var method in methods)
                {
                    if (UpdateMethodNames.Contains(method.Identifier.ValueText))
                    {
                        callback(method);
                    }
                }
            }
        }

        public bool IsMonoBehaviour()
        {
            return IsMonoBehavior(_classSymbol);
        }

        private static bool IsMonoBehavior(INamedTypeSymbol classDeclaration)
        {
            if (classDeclaration.BaseType == null)
            {
                return false;
            }

            var baseClass = classDeclaration.BaseType;

            if (baseClass.ContainingNamespace.Name.Equals("UnityEngine") && baseClass.Name.Equals("MonoBehaviour"))
            {
                return true;
            }

            return IsMonoBehavior(baseClass); //determine if the BaseClass extends mono behavior

        }
    }
}
