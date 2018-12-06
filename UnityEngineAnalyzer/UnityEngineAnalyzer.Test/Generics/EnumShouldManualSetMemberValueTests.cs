using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using RoslynNUnitLight;
using UnityEngineAnalyzer.Generics;


//using Microsoft.CodeAnalysis.Workspaces;

namespace UnityEngineAnalyzer.Test.GCAlloc
{
    [TestFixture]
    sealed class EnumShouldManualSetMemberValueTests : AnalyzerTestFixture
    {

        protected override string LanguageName => LanguageNames.CSharp;
        protected override DiagnosticAnalyzer CreateAnalyzer() => new EnumShouldManualSetMemberValue();

        [Test]
        public void EnumWithoutManualMemberValue()
        {
            var code = @"

enum E
{
    [|a|],
    b = 2,
}

";

            Document document;
            TextSpan span;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, null,
                out document, out span))
            {
                HasDiagnostic(document, span, DiagnosticIDs.EnumShouldManualSetMemberValue);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }

        

        [Test]
        public void EnumWithManualMemberValue()
        {
            var code = @"

enum E
{
    [|a|] = 1,
    b = 2,
}
";

            Document document;
            TextSpan span;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, null,
                out document, out span))
            {
                NoDiagnostic(document, DiagnosticIDs.EnumShouldManualSetMemberValue);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }
    }
}
