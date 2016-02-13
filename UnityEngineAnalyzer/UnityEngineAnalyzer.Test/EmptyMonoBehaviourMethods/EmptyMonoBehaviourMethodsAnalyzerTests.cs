using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using RoslynNUnitLight;
using UnityEngineAnalyzer.EmptyMonoBehaviourMethods;

namespace UnityEngineAnalyzer.Test.EmptyMonoBehaviourMethods
{
    [TestFixture]
    sealed class EmptyMonoBehaviourMethodsAnalyzerTests : AnalyzerTestFixture
    {
        protected override string LanguageName => LanguageNames.CSharp;
        protected override DiagnosticAnalyzer CreateAnalyzer() => new EmptyMonoBehaviourMethodsAnalyzer();

        [Test]
        public void EmptyUpdateInMonoBehaviour()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    [|void Update() { }|]
}";
            Document document;
            TextSpan span;
            TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine, out document, out span);

            HasDiagnostic(document, span, DiagnosticIDs.EmptyMonoBehaviourMethod);
        }

        [Test]
        public void EmptyUpdateInNormalClass()
        {
            const string code = @"
using UnityEngine;

class C
{
    [|void Update() { }|]
}";
            Document document;
            TextSpan span;
            TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine, out document, out span);

            NoDiagnostic(document, DiagnosticIDs.EmptyMonoBehaviourMethod);
        }
    }
}
