using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using RoslynNUnitLight;
using UnityEngineAnalyzer.AOT;

namespace UnityEngineAnalyzer.Test.AOT
{
    [TestFixture]
    sealed class TypeGetTypeAnalyzerTests : AnalyzerTestFixture
    {
        protected override string LanguageName => LanguageNames.CSharp;
        protected override DiagnosticAnalyzer CreateAnalyzer() => new TypeGetTypeAnalyzer();

        [Test]
        public void TypeGetTypeIsUsed()
        {
            const string code = @"
using UnityEngine;
using System;

class C : MonoBehaviour
{
    void Start()
    {
        var theType =  [|Type.GetType("""")|];
    }
}";
            Document document;
            TextSpan span;

            var references = MetadataReferenceHelper.UsingUnityEngine;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, references, out document, out span))
            {
                this.HasDiagnostic(document, span, DiagnosticIDs.TypeGetType);
            }
            else
            {
                Assert.Fail("Could not load the Test code in the unit test");
            }
            


        }
    }
}
