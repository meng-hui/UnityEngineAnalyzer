using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using RoslynNUnitLight;
using UnityEngineAnalyzer.StringMethods;

namespace UnityEngineAnalyzer.Test.StringMethods
{
    [TestFixture]
    sealed class InvokeFunctionMissingAnalyzerTests : AnalyzerTestFixture
    {
        protected override string LanguageName => LanguageNames.CSharp;
        protected override DiagnosticAnalyzer CreateAnalyzer() => new InvokeFunctionMissingAnalyzer();

        [Test]
        public void InvokeUsedInMonoBehaviourClass()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    void Start() { [|Invoke(string.Empty, 0f)|]; }
}";

            Document document;
            TextSpan span;
            TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine, out document, out span);

            HasDiagnostic(document, span, DiagnosticIDs.InvokeFunctionMissing);
        }

        [Test]
        public void InvokeUsedByMonoBehaviourClass()
        {
            const string code = @"
using UnityEngine;

class CC : MonoBehaviour { }

class C : MonoBehaviour
{
    private CC cc;
    void Start() { [|cc.Invoke(string.Empty, 0f)|]; }
}";

            Document document;
            TextSpan span;
            TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine, out document, out span);

            HasDiagnostic(document, span, DiagnosticIDs.InvokeFunctionMissing);
        }

        [Test]
        public void InvokeRepeatingUsedInMonoBehaviourClass()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    void Start() { [|InvokeRepeating(string.Empty, 0f, 0f)|]; }
}";

            Document document;
            TextSpan span;
            TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine, out document, out span);

            HasDiagnostic(document, span, DiagnosticIDs.InvokeFunctionMissing);
        }

        [Test]
        public void InvokeRepeatingUsedByMonoBehaviourClass()
        {
            const string code = @"
using UnityEngine;

class CC : MonoBehaviour { }

class C : MonoBehaviour
{
    private CC cc;
    void Start() { [|cc.InvokeRepeating(string.Empty, 0f, 0f)|]; }
}";

            Document document;
            TextSpan span;
            TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine, out document, out span);

            HasDiagnostic(document, span, DiagnosticIDs.InvokeFunctionMissing);
        }
    }
}
