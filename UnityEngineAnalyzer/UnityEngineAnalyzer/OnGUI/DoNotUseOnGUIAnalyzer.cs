using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace UnityEngineAnalyzer.OnGUI
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DoNotUseOnGUIAnalyzer : DiagnosticAnalyzer
    {   
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DiagnosticDescriptors.DoNotUseOnGUI);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Method);
        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            // check if the method is name OnGUI
            var methodSymbol = context.Symbol as IMethodSymbol;
            if (!methodSymbol.Name.Equals("OnGUI")) { return; }

            // check that it is contained in a class extended by UnityEngine.MonoBehaviour
            var containingClass = methodSymbol.ContainingType;
            var baseClass = containingClass.BaseType;
            if (baseClass.ContainingNamespace.Name.Equals("UnityEngine") &&
                baseClass.Name.Equals("MonoBehaviour"))
            {
                var diagnostic = Diagnostic.Create(DiagnosticDescriptors.DoNotUseOnGUI, methodSymbol.Locations[0], containingClass.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
