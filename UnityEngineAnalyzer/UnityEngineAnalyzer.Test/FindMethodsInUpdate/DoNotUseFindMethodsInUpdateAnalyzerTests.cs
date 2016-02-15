using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using RoslynNUnitLight;
using UnityEngineAnalyzer.FindMethodsInUpdate;

namespace UnityEngineAnalyzer.Test.FindMethodsInUpdate
{
    [TestFixture]
    sealed class DoNotUseFindMethodsInUpdateAnalyzerTests : AnalyzerTestFixture
    {
        protected override string LanguageName => LanguageNames.CSharp;
        protected override DiagnosticAnalyzer CreateAnalyzer() => new DoNotUseFindMethodsInUpdateAnalyzer();

        [Test]
        public void GameObjectFindInUpdate()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    void Update() 
    {
        [|GameObject.Find("")|]; 
    }
}";
            Document document;
            TextSpan span;
            TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine, out document, out span);

            HasDiagnostic(document, span, DiagnosticIDs.EmptyMonoBehaviourMethod);
        }

        [Test]
        public void GameObjectFindInStart()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    void Start() 
    {
        [|GameObject.Find("")|]; 
    }
}";
            Document document;
            TextSpan span;
            TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine, out document, out span);

            NoDiagnostic(document, DiagnosticIDs.EmptyMonoBehaviourMethod);
        }
    }
}
