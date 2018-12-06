using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace UnityEngineAnalyzer.Generics
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class EnumShouldManualSetMemberValue : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DiagnosticDescriptors.EnumShouldManualSetMemberValue);
        
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.EnumMemberDeclaration);
        }
        

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var targetSyntax = context.Node as EnumMemberDeclarationSyntax;
            if (targetSyntax == null)
            {
                return;
            }

            bool hasEqualClause = false;
            foreach(var oneSyntax in targetSyntax.DescendantNodes().OfType<EqualsValueClauseSyntax>())
            {
                if(oneSyntax != null)
                {
                    hasEqualClause = true;
                    break;
                }
            }

            if(!hasEqualClause)
            {
                var diagnostic = Diagnostic.Create(DiagnosticDescriptors.EnumShouldManualSetMemberValue,
                    targetSyntax.GetLocation(), targetSyntax.ToString());
                context.ReportDiagnostic(diagnostic);
            }

        }
    }
}
