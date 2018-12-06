using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UnityEngineAnalyzer.EmptyMonoBehaviourMethods
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EmptyMonoBehaviourMethodsCodeFixer)), Shared]
    public class EmptyMonoBehaviourMethodsCodeFixer : CodeFixProvider
    {
        private const string _title = "Remove Method";

        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(DiagnosticIDs.EmptyMonoBehaviourMethod);

        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf()
                .OfType<MethodDeclarationSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    _title,
                    c => RemveMethod(context.Document, declaration, c),
                    _title
                    ),
                    diagnostic
                );
        }

        private async Task<Document> RemveMethod(Document document, MethodDeclarationSyntax declaration, CancellationToken cancellationToken)
        {
            var removedParent = declaration.Parent.RemoveNode(declaration, SyntaxRemoveOptions.KeepNoTrivia);
            var oldRoot = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = oldRoot.ReplaceNode(declaration.Parent, removedParent);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
