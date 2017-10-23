using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using RoslynNUnitLight;
using UnityEngineAnalyzer.Physics;

namespace UnityEngineAnalyzer.Test.Animator
{

    [TestFixture]
    sealed class UseNonAllocMethodsTests : AnalyzerTestFixture
    {
        protected override string LanguageName => LanguageNames.CSharp;
        protected override DiagnosticAnalyzer CreateAnalyzer() => new UseNonAllocMethodsAnalyzer();

        [Test]
        public void PhysicsRaycastAll()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    void Update()
    {
        RaycastHit[] hits;
        hits = [|Physics.RaycastAll(transform.position, transform.forward, 100.0F)|];
    }
}";

            Document document;
            TextSpan span;
            TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine, out document, out span);

            HasDiagnostic(document, span, DiagnosticIDs.PhysicsUseNonAllocMethods);
        }

        [Test]
        public void Physics2DRaycastAll()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    void Update()
    {
        RaycastHit2D[] hits;
        hits = [|Physics2D.RaycastAll(Vector2.zero, Vector2.one)|];
    }
}";

            Document document;
            TextSpan span;
            TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine, out document, out span);

            HasDiagnostic(document, span, DiagnosticIDs.PhysicsUseNonAllocMethods);
        }

        [Test]
        public void Physics2DCircleCastAll()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    void Start()
    {
        [|Physics2D.CircleCastAll(Vector2.zero, 5, Vector2.one)|];
    }
}";

            Document document;
            TextSpan span;
            TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine, out document, out span);

            HasDiagnostic(document, span, DiagnosticIDs.PhysicsUseNonAllocMethods);
        }

        [Test]
        public void PhysicsUseNonAllocMethods_Should_Notice_Multiple_Calls()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    Animator animator;

    void Start()
    {
        [|Physics2D.CircleCastAll(Vector2.zero, 5, Vector2.one)|];

        [|Physics2D.LinecastAll(Vector2.zero, Vector2.one)|];
    }
}";

            Document document;
            TextSpan span;
            TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine, out document, out span);

            HasDiagnostic(document, span, DiagnosticIDs.PhysicsUseNonAllocMethods);
        }
    }
}
