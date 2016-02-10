using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using RoslynNUnitLight;
using UnityEngineAnalyzer.OnGUI;

namespace UnityEngineAnalyzer.Test.OnGUI
{
    [TestFixture]
    sealed class DoNotUseOnGUIAnalyzerTests : AnalyzerTestFixture
    {
        protected override string LanguageName => LanguageNames.CSharp;
        protected override DiagnosticAnalyzer CreateAnalyzer() => new DoNotUseOnGUIAnalyzer();

        [Test]
        public void OnGUIUsedInMonoBehaviour()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    void [|OnGUI|]() { }
}";

            Document document;
            TextSpan span;
            TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine, out document, out span);

            HasDiagnostic(document, span, DiagnosticIDs.DoNotUseOnGUI);
        }

        [Test]
        public void OnGUIUsedInClass()
        {
            const string code = @"
class C
{
    void [|OnGUI|]() { }
}";

            NoDiagnostic(code, DiagnosticIDs.DoNotUseOnGUI);
        }
    }
}
