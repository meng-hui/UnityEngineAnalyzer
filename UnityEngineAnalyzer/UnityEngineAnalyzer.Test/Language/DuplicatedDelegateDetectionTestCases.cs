using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using RoslynNUnitLight;
using UnityEngineAnalyzer.Language;

namespace UnityEngineAnalyzer.Test.Language
{
    [TestFixture]
    class DuplicatedDelegateDetectionTestCases:AnalyzerTestFixture
    {
        protected override string LanguageName => LanguageNames.CSharp;
        protected override DiagnosticAnalyzer CreateAnalyzer()=>new DuplicatedDelegateDetection();

        [Test]
        public void DetectCustomedDelegateDeclaration()
        {
            var code = @"
class TestHost
{
    [|private delegate void TestDelegate(string argu1);|]
}
";
            Document document;
            TextSpan span;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName,
                MetadataReferenceHelper.UsingUnityEngine, out document, out span))
            {
                //    HasDiagnostic(document, span, DiagnosticIDs.UseCommonDelegate);
                HasDiagnostic(document, span, DiagnosticIDs.UseCommonDelegate);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }

        [Test]
        public void DetectUDTDelegateDeclaration()
        {
            var code = @"
class TestHost
{
    [|private delegate TestHost TestDelegate(TestHost argu1);|]
}
";
            Document document;
            TextSpan span;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName,
                MetadataReferenceHelper.UsingUnityEngine, out document, out span))
            {
                //    HasDiagnostic(document, span, DiagnosticIDs.UseCommonDelegate);
                HasDiagnostic(document, span, DiagnosticIDs.UseCommonDelegate);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }

        [Test]
        public void DetectUDTDelegateDeclarationWithOutKeyword()
        {
            var code = @"
class TestHost
{
    [|private delegate TestHost TestDelegate(TestHost argu1, out int arg2);|]
}
";
            Document document;
            TextSpan span;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName,
                MetadataReferenceHelper.UsingUnityEngine, out document, out span))
            {
                //    HasDiagnostic(document, span, DiagnosticIDs.UseCommonDelegate);
                HasDiagnostic(document, span, DiagnosticIDs.UseCommonDelegate);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }

        [Test]
        public void DetectDelegateAsParam()
        {
            var code = @"
class TestHost
{
    [|private delegate string TestDelegate(string argu);|]

    private void TestMethod([|TestDelegate argu|])
    {
    }
}
";
            Document document;
            TextSpan span;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName,
                MetadataReferenceHelper.UsingUnityEngine, out document, out span))
            {
                HasDiagnostic(document, span, DiagnosticIDs.UseCommonDelegate);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }

        [Test]
        public void NoDetectDelegateAsParam()
        {
            var code = @"
using System;

class TestHost
{
    private void TestMethod([|Action argu|])
    {
    }
}
";
            Document document;
            TextSpan span;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName,
                MetadataReferenceHelper.UsingUnityEngine, out document, out span))
            {
                NoDiagnostic(document, DiagnosticIDs.UseCommonDelegate);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }

        [Test]
        public void DetectDelegateAsVariable()
        {
            var code = @"
using System;

class TestHost
{
    [|private delegate void Miaow(string str);|]

    private [|Miaow|] aa;
}
";

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName,
                MetadataReferenceHelper.UsingUnityEngine, out var document, out var span))
            {
                HasDiagnostic(document, span, DiagnosticIDs.UseCommonDelegate);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }

        [Test]
        public void NoDetectDelegateAsVariable()
        {
            var code = @"
using System;

class TestHost
{
    [|private Action<string> aa;|]
}
";

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName,
                MetadataReferenceHelper.UsingUnityEngine, out var document, out _))
            {
                NoDiagnostic(document, DiagnosticIDs.UseCommonDelegate);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }
    }
}
