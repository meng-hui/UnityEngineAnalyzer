﻿using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace UnityEngineAnalyzer
{
    public static class RolsynExtensions
    {
        public static bool TryGetSymbolInfo(this SyntaxNodeAnalysisContext context, SyntaxNode node, out SymbolInfo symbolInfo)
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

        public static bool IsDerived(this ClassDeclarationSyntax classDeclaration)
        {
            return (classDeclaration.BaseList != null && classDeclaration.BaseList.Types.Count > 0);
        }

        public static bool IsSealed(this ClassDeclarationSyntax classDeclaration)
        { 
            return classDeclaration.Modifiers.Any(m => m.Kind() == SyntaxKind.SealedKeyword);
        }

        public static bool IsSealed(this MethodDeclarationSyntax methodDeclaration)
        {
            return methodDeclaration.Modifiers.Any(m => m.Kind() == SyntaxKind.SealedKeyword);
        }

        public static bool IsOverriden(this MethodDeclarationSyntax methodDeclaration)
        {
            return methodDeclaration.Modifiers.Any(m => m.Kind() == SyntaxKind.OverrideKeyword);
        }

        public static string MethodName(this InvocationExpressionSyntax invocation)
        {

            string name;

            if (invocation.Expression is MemberAccessExpressionSyntax)
            {
                name = ((MemberAccessExpressionSyntax)invocation.Expression).Name.Identifier.ToString();
            }
            else if (invocation.Expression is IdentifierNameSyntax)
            {
                name = ((IdentifierNameSyntax)invocation.Expression).ToString();
            }
            else if (invocation.Expression is GenericNameSyntax)
            {
                name = ((GenericNameSyntax)invocation.Expression).Identifier.ToString();
            }
            else
            {
                throw new ArgumentException("Unable to determine name of method");
            }
            return name;
        }

        public static T GetArgumentValue<T>(this ArgumentSyntax argument)
        {
            //NOTE: Possibly add support for constant parameters in the future

            if (argument.Expression is LiteralExpressionSyntax)
            {
                var argumentValue = ((LiteralExpressionSyntax)argument.Expression).Token.Value;
                return (T)argumentValue;
            }
            return default(T);
        }
    }
}
