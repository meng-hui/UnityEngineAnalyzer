using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using RoslynNUnitLight;
using UnityEngineAnalyzer.Coroutines;

namespace UnityEngineAnalyzer.Test.Coroutines
{
    [TestFixture]
    sealed class DoNotUseCoroutinesAnalyzerTests : AnalyzerTestFixture
    {
        protected override string LanguageName => LanguageNames.CSharp;
        protected override DiagnosticAnalyzer CreateAnalyzer() => new DoNotUseCoroutinesAnalyzer();

        [Test]
        public void StartCoroutineUsedInMonoBehaviourClass()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    void M()
    {
        [|StartCoroutine(""MyCoroutine"")|];
    }
}";
            Document document;
            TextSpan span;
            TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine, out document, out span);

            HasDiagnostic(document, span, DiagnosticIDs.DoNotUseCoroutines);
        }

        [Test]
        public void StartCoroutineUsedByMonoBehaviourClass()
        {
            const string code = @"
using UnityEngine;

class CC : MonoBehaviour { }

class C : MonoBehaviour
{
    private CC cc;
    void M()
    {
        [|cc.StartCoroutine(""MyCoroutine"")|];
    }
}";
            Document document;
            TextSpan span;
            TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine, out document, out span);

            HasDiagnostic(document, span, DiagnosticIDs.DoNotUseCoroutines);
        }
    }
}
