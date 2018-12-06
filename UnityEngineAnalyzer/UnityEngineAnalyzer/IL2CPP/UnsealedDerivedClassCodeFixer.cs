using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UnityEngineAnalyzer.IL2CPP
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UnsealedDerivedClassCodeFixer)), Shared]
    public class UnsealedDerivedClassCodeFixer:CodeFixProvider
    {
        private const string _title = "Add sealed Modifier";

        public override ImmutableArray<string> FixableDiagnosticIds =>ImmutableArray.Create(DiagnosticIDs.UnsealedDerivedClass);

        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf()
                .OfType<MethodDeclarationSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    _title,
                    c=>AddModifier(context.Document, declarations, c),
                    _title
                    ),
                diagnostic
            );
        }

        private async Task<Document> AddModifier(Document contextDocument, MethodDeclarationSyntax declaration, CancellationToken cancellationToken)
        {
            var firstToken = declaration.GetFirstToken();
            var leadingTrivia = firstToken.LeadingTrivia;
            //var trimmedDeclaration =
            //    declaration.ReplaceToken(firstToken, firstToken.WithLeadingTrivia(SyntaxTriviaList.Empty));

            var sealedToken = SyntaxFactory.Token(leadingTrivia, SyntaxKind.SealedKeyword,
                SyntaxFactory.TriviaList(SyntaxFactory.ElasticMarker));

            var newModifiers = declaration.Modifiers.Insert(declaration.Modifiers.Count >= 1 ? 1 : 0, sealedToken);
            var newDecolation = declaration.WithModifiers(newModifiers);

            var oldRoot = await contextDocument.GetSyntaxRootAsync(cancellationToken);
            var newRoot = oldRoot.ReplaceNode(declaration, newDecolation);

            return contextDocument.WithSyntaxRoot(newRoot);
        }
    }
}
