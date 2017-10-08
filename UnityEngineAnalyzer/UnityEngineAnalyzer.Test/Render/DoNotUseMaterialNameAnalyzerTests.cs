using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using RoslynNUnitLight;
using UnityEngineAnalyzer.Render;

namespace UnityEngineAnalyzer.Test.Render
{

    [TestFixture]
    sealed class DoNotUseMaterialNameAnalyzerTests : AnalyzerTestFixture
    {
        protected override string LanguageName => LanguageNames.CSharp;
        protected override DiagnosticAnalyzer CreateAnalyzer() => new DoNotUseMaterialNameAnalyzer();

        [Test]
        public void MaterialSetFloatStringName()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    Material mat;

    void Start()
    {
        [|mat.SetFloat(""Run"", 1.2f)|];
    }
}";

            Document document;
            TextSpan span;
            TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine, out document, out span);

            HasDiagnostic(document, span, DiagnosticIDs.DoNotUseMaterialNameInMaterial);
        }
        
    }
}
