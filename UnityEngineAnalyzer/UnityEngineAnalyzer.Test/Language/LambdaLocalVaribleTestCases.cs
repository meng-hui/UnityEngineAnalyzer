using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using RoslynNUnitLight;
using UnityEngineAnalyzer.Language;

namespace UnityEngineAnalyzer.Test.Language
{
    [TestFixture]
    class LambdaLocalVaribleTestCases:AnalyzerTestFixture
    {
        protected override string LanguageName => LanguageNames.CSharp;
        protected override DiagnosticAnalyzer CreateAnalyzer()=>new LambdaClosureAnalyzer();

        [Test]
        public void ExpressionWithConstructClosure()
        {
            var code = @"
using System;

class TestHost
{
    private void QueryLambdaArgu(Action callback)
    {
        callback();
    }

    public void TestMethod()
    {
        int a;
        QueryLambdaArgu(() => {
            int b = 999;
            int c = [|a|];
            [|a|].ToString();
            b.ToString();
            b.ToString();
        });
    }
}";
            Document document;
            TextSpan span;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName,
                MetadataReferenceHelper.UsingUnityEngine, out document, out span))
            {
                HasDiagnostic(document, span, DiagnosticIDs.LambdaUseLocalVariable);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }

        [Test]
        public void ExpressionWithoutConstructClosure()
        {
            var code = @"
using System;

class TestHost
{
    private void QueryLambdaArgu(Action callback)
    {
        callback();
    }

    public void TestMethod()
    {
        int a;
        QueryLambdaArgu([|() => {
            int b = 999;
            b.ToString();
            b.ToString();
        }|]);
    }
}";
            Document document;
            TextSpan span;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName,
                MetadataReferenceHelper.UsingUnityEngine, out document, out span))
            {
                NoDiagnostic(document, DiagnosticIDs.LambdaUseLocalVariable);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }
    }
}
