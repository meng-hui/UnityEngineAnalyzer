using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using RoslynNUnitLight;
using UnityEngineAnalyzer.FindMethodsInUpdate;


//using Microsoft.CodeAnalysis.Workspaces;

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
            var code = @"
using UnityEngine;

class C : MonoBehaviour
{
    void Update()
    {
        [|GameObject.Find(""param"")|];

        //var result = GameObject.Find(""param"");
    }
}";

            Document document;
            TextSpan span;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine,
                out document, out span))
            {
                HasDiagnostic(document, span, DiagnosticIDs.DoNotUseFindMethodsInUpdate);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
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
