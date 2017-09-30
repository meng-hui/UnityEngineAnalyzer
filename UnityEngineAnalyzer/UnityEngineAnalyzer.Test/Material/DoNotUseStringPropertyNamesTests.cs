using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using RoslynNUnitLight;
using UnityEngineAnalyzer.Material;

namespace UnityEngineAnalyzer.Test.Material
{

    [TestFixture]
    sealed class DoNotUseStringPropertyNamesAnalyzerTests : AnalyzerTestFixture
    {
        protected override string LanguageName => LanguageNames.CSharp;
        protected override DiagnosticAnalyzer CreateAnalyzer() => new DoNotUseStringPropertyNamesAnalyzer();

        [Test]
        public void MaterialGetFloatWithStringProperty()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    Material material;

    void Start()
    {
        [|material.GetFloat(""_Shininess"")|];
    }
}";

            Document document;
            TextSpan span;
            TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine, out document, out span);

            HasDiagnostic(document, span, DiagnosticIDs.DoNotUseStringPropertyNamesInMaterial);
        }

        [Test]
        public void MaterialSetFloatWithStringProperty()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    Material material;

    void Start()
    {
        [|material.SetFloat(""_Shininess"", 1.2f)|];
    }
}";

            Document document;
            TextSpan span;
            TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine, out document, out span);

            HasDiagnostic(document, span, DiagnosticIDs.DoNotUseStringPropertyNamesInMaterial);
        }

        [Test]
        public void MaterialSetMatrixWithStringProperty()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    Material material;

    void Start()
    {
        [|material.SetVector(""_WaveAndDistance"", Vector3.one)|];
    }
}";

            Document document;
            TextSpan span;
            TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine, out document, out span);

            HasDiagnostic(document, span, DiagnosticIDs.DoNotUseStringPropertyNamesInMaterial);
        }
    }
}
