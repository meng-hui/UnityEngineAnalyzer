using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using RoslynNUnitLight;
using UnityEngineAnalyzer.CompareTag;

namespace UnityEngineAnalyzer.Test.CompareTag
{
    [TestFixture]
    sealed class UseCompareTagAnalyzerTests : AnalyzerTestFixture
    {
        protected override string LanguageName => LanguageNames.CSharp;
        protected override DiagnosticAnalyzer CreateAnalyzer() => new UseCompareTagAnalyzer();

        [Test]
        public void UseTagForComparison()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    void Start()
    {
        var a = [|tag == ""Enemy""|];
    }
}";

            Document document;
            TextSpan span;
            TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine, out document, out span);

            HasDiagnostic(document, span, DiagnosticIDs.UseCompareTag);
        }

        [Test]
        public void UseGameObjectTagForComparison()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    void Start()
    {
        var a = [|gameObject.tag == ""Enemy""|];
    }
}";

            Document document;
            TextSpan span;
            TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine, out document, out span);

            HasDiagnostic(document, span, DiagnosticIDs.UseCompareTag);
        }

        [Test]
        public void UseMonoBehaviourTagForComparison()
        {
            const string code = @"
using UnityEngine;

class CC : MonoBehaviour { }

class C : MonoBehaviour
{
    private CC cc;
    void Start()
    {
        var a = [|""Enemy"".Equals(cc.tag)|];
    }
}";

            Document document;
            TextSpan span;
            TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine, out document, out span);

            HasDiagnostic(document, span, DiagnosticIDs.UseCompareTag);
        }

        [Test]
        public void OtherEqualsComparison()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    void Start()
    {
        var a = [|""Player"".Equals(""Enemy"")|];
    }
}";

            Document document;
            TextSpan span;
            TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine, out document, out span);

            NoDiagnostic(document, DiagnosticIDs.UseCompareTag);
        }
    }
}
