using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace UnityEngineAnalyzer.Language
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DuplicatedDelegateDetection : DiagnosticAnalyzer
    {
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(
                AnalysisDelegate, 
                SyntaxKind.DelegateDeclaration);
            context.RegisterSyntaxNodeAction(
                AnalysisDelegateArgument,
                SyntaxKind.Parameter);
            context.RegisterSyntaxNodeAction(
                AnalysisDeclaration,
                SyntaxKind.VariableDeclaration);
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(
                DiagnosticDescriptors.DuplicateDelegateDetection,
                DiagnosticDescriptors.DuplicateDelegateParamDetection
                );

        private void AnalysisDelegateArgument(SyntaxNodeAnalysisContext context)
        {
            var paramNode = (ParameterSyntax)context.Node;
            if (paramNode.Type == null) return;
            var paramTypeSymbol = context.SemanticModel?.GetSymbolInfo(paramNode.Type).Symbol;

            if (paramTypeSymbol == null) return;

            if (paramTypeSymbol is ITypeSymbol typeSymbol)
            {
                if(IsDelegate(typeSymbol))
                    context.ReportDiagnostic(Diagnostic.Create(
                        SupportedDiagnostics[1],
                        paramNode.GetLocation(),
                        paramTypeSymbol.MetadataName));
            }
        }

        private void AnalysisDeclaration(SyntaxNodeAnalysisContext context)
        {
            var declaration = (VariableDeclarationSyntax) context.Node;
            if (declaration.Type == null) return;
            var paramTypeSymbol = context.SemanticModel?.GetSymbolInfo(declaration.Type).Symbol;

            if (paramTypeSymbol == null) return;

            if (paramTypeSymbol is ITypeSymbol typeSymbol)
            {
                if (IsDelegate(typeSymbol))
                    context.ReportDiagnostic(Diagnostic.Create(
                        SupportedDiagnostics[1],
                        declaration.Type.GetLocation(),
                        paramTypeSymbol.MetadataName));
            }
        }

        private void AnalysisDelegate(SyntaxNodeAnalysisContext context)
        {
            var declaration = (DelegateDeclarationSyntax) context.Node;
            if (declaration.ReturnType == null) return;
            var returnSymbol = context.SemanticModel?.GetSymbolInfo(declaration.ReturnType).Symbol.MetadataName;

            if (returnSymbol == null) return;

            var paramsSymbols =
                (
                from paramsInfo in declaration.ParameterList.Parameters
                select context.SemanticModel.GetSymbolInfo(paramsInfo.Type).Symbol.MetadataName
                ).ToList();

            var baseType = new StringBuilder(returnSymbol == "Void" ? "Action" : "Func");
            if(returnSymbol !="Void")
                paramsSymbols.Insert(0, returnSymbol);

            if (paramsSymbols.Count > 0)
            {
                baseType.AppendFormat("<{0}>", string.Join(", ", paramsSymbols));
            }

            context.ReportDiagnostic(Diagnostic.Create(
                SupportedDiagnostics.First(),
                declaration.GetLocation(),
                baseType.ToString()));
        }

        private static bool IsDelegate(ITypeSymbol typeSymbol)
        {
            if (typeSymbol == null) return false;

            var currentType = typeSymbol;
            while (currentType.MetadataName != "Object")
            {
                if (currentType.MetadataName.StartsWith("Func") ||
                    currentType.MetadataName.StartsWith("Action"))
                    return false;

                if (currentType.MetadataName == "Delegate")
                {
                    return true;
                }
                currentType = currentType.BaseType;
                if (currentType == null)
                    return false;
            }

            return false;
        }
    }
}
