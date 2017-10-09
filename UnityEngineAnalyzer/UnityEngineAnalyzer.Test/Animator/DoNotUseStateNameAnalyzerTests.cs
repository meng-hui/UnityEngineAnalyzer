using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using RoslynNUnitLight;
using UnityEngineAnalyzer.Animator;

namespace UnityEngineAnalyzer.Test.Animator
{

    [TestFixture]
    sealed class DoNotSetAnimatorParameterWithNameAnalyzerTests : AnalyzerTestFixture
    {
        protected override string LanguageName => LanguageNames.CSharp;
        protected override DiagnosticAnalyzer CreateAnalyzer() => new DoNotUseStateNameAnalyzer();

        [Test]
        public void AnimatorSetFloatStringName()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    Animator animator;

    void Start()
    {
        [|animator.SetFloat(""Run"", 1.2f)|];
    }
}";

            Document document;
            TextSpan span;
            TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine, out document, out span);

            HasDiagnostic(document, span, DiagnosticIDs.DoNotUseStateNameInAnimator);
        }

        [Test]
        public void AnimatorSetIntStringName()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    Animator animator;

    void Start()
    {
        [|animator.SetInteger(""Walk"", 1)|];
    }
}";

            Document document;
            TextSpan span;
            TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine, out document, out span);

            HasDiagnostic(document, span, DiagnosticIDs.DoNotUseStateNameInAnimator);
        }

        [Test]
        public void AnimatorSetBoolStringName()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    Animator animator;

    void Start()
    {
        [|animator.SetBool(""Fly"", true)|];
    }
}";

            Document document;
            TextSpan span;
            TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine, out document, out span);

            HasDiagnostic(document, span, DiagnosticIDs.DoNotUseStateNameInAnimator);
        }
    }
}
