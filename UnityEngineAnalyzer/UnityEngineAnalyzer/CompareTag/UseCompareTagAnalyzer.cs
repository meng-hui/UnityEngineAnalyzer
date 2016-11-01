using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace UnityEngineAnalyzer.CompareTag
{
    //TODO: Create a CodeFix provider for this

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class UseCompareTagAnalyzer : DiagnosticAnalyzer
    {
        private static readonly ImmutableHashSet<string> ContainingSymbols = ImmutableHashSet.Create("UnityEngine.Component", "UnityEngine.GameObject");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DiagnosticDescriptors.UseCompareTag);
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeInvocationExpressionNode, SyntaxKind.InvocationExpression);
            context.RegisterSyntaxNodeAction(AnalyzeBinaryExpressionNode, SyntaxKind.EqualsExpression, SyntaxKind.NotEqualsExpression);
        }

        private static void AnalyzeInvocationExpressionNode(SyntaxNodeAnalysisContext context)
        {
            // look for an InvocationExpressionSyntax of the form tag.Equals("") or "".Equals(tag)
            var invocationExpression = context.Node as InvocationExpressionSyntax;
            // check that number of arguments is one
            if (invocationExpression.ArgumentList?.Arguments.Count != 1) { return; }
            // retrieve the MemberAccessExpression and check that is it "Equals", check that number of arguments is one
            var equalsMemberAccessExpression = invocationExpression.Expression as MemberAccessExpressionSyntax;
            if (!(equalsMemberAccessExpression?.Name.Identifier.Text.Equals("Equals") ?? false)) { return; }

            // at this point we have an .Equals member access with one argument
            // check on both sides if there is a tag member access
            if (ProcessPotentialTagMemberAccessExpression(context, equalsMemberAccessExpression.Expression) ||
                ProcessPotentialTagMemberAccessExpression(context, invocationExpression.ArgumentList.Arguments[0].Expression))
            {
                ReportDiagnostic(context, invocationExpression.GetLocation());
            }
        }

        private static void AnalyzeBinaryExpressionNode(SyntaxNodeAnalysisContext context)
        {
            // look for tag == "" or tag != ""
            var binaryExpression = context.Node as BinaryExpressionSyntax;
            if (ProcessPotentialTagMemberAccessExpression(context, binaryExpression.Left) ||
                ProcessPotentialTagMemberAccessExpression(context, binaryExpression.Right))
            {
                ReportDiagnostic(context, binaryExpression.GetLocation());
            }
        }

        private static bool ProcessPotentialTagMemberAccessExpression(SyntaxNodeAnalysisContext context, ExpressionSyntax expression)
        {
            // check for member access or identifier access
            if (expression is MemberAccessExpressionSyntax || expression is IdentifierNameSyntax)
            {
                // check for property access
                var propertySymbol = context.SemanticModel.GetSymbolInfo(expression).Symbol as IPropertySymbol;
                // check that property accessed is tag and belongs to UnityEngine
                if (propertySymbol?.Name.Equals("tag") ?? false && //TODO: Either fix this statement or remove this check
                    ContainingSymbols.Contains(propertySymbol.ContainingSymbol.ToString()))
                { return true; }
            }

            return false;
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, Location location)
        {
            var diagnostic = Diagnostic.Create(DiagnosticDescriptors.UseCompareTag, location);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
