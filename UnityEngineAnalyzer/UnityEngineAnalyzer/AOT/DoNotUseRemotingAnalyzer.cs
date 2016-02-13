using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace UnityEngineAnalyzer.AOT
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DoNotUseRemotingAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DiagnosticDescriptors.DoNotUseRemoting);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.UsingDirective);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            // retrieve using syntax node
            var syntax = context.Node as UsingDirectiveSyntax;
            // and check if it is System.Runtime.Remoting
            if (syntax.Name.ToString().Equals("System.Runtime.Remoting"))
            {
                var diagnostic = Diagnostic.Create(DiagnosticDescriptors.DoNotUseRemoting, syntax.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
