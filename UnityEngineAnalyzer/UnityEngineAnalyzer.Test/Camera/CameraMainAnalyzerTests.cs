using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using RoslynNUnitLight;
using UnityEngineAnalyzer.Camera;

namespace UnityEngineAnalyzer.Test.Camera
{
    [TestFixture]
    sealed class CameraMainAnalyzerTests : AnalyzerTestFixture
    {
        protected override string LanguageName => LanguageNames.CSharp;
        protected override DiagnosticAnalyzer CreateAnalyzer() => new CameraMainAnalyzer();

        [Test]
        public void CameraMainShouldRaiseWarningOnMemberExpression()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    void Update()
    {
        var orthographicSize = [|Camera.main.orthographicSize|];
    }
}";

            Document document;
            TextSpan span;
            TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine, out document, out span);

            HasDiagnostic(document, span, DiagnosticIDs.CameraMainIsSlow);
        }

        [Test]
        public void CameraMainShouldRaiseWarningOnMethod()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    void Update()
    {
        var size = [|Camera.main.ScreenPointToRay|](new Vector3(200, 200, 0));
    }
}";

            Document document;
            TextSpan span;
            TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine, out document, out span);

            HasDiagnostic(document, span, DiagnosticIDs.CameraMainIsSlow);
        }

        [Test]
        public void CameraMainShouldThrowWarningFromCalledMethods()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    void Update()
    {
        Call();

        //Camera.main.transform.position = Vector3.one;
    }

    public void Call() 
    {
        var size = [|Camera.main.ScreenPointToRay|](new Vector3(200, 200, 0));
    }
}";

            Document document;
            TextSpan span;
            TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine, out document, out span);

            HasDiagnostic(document, span, DiagnosticIDs.CameraMainIsSlow);
        }

        [Test]
        public void CameraMainShouldThrowWarningOnlyInHotPath()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    void Start()
    {
        var size = [|Camera.main.ScreenPointToRay|](new Vector3(200, 200, 0));
    }
}";

            Document document;
            TextSpan span;
            TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine, out document, out span);

            NoDiagnostic(document, DiagnosticIDs.CameraMainIsSlow);
        }
    }
}
