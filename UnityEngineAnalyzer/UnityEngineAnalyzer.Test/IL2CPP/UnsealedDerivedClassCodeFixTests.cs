using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;
using NUnit.Core;
using NUnit.Framework;
using RoslynNUnitLight;
using UnityEngineAnalyzer.IL2CPP;

namespace UnityEngineAnalyzer.Test.IL2CPP
{
    [TestFixture]
    sealed class UnsealedDerivedClassCodeFixTests:CodeFixTestFixture
    {
        protected override string LanguageName => LanguageNames.CSharp;
        protected override  CodeFixProvider CreateProvider()=>new UnsealedDerivedClassCodeFixer();

        [Test]
        public void AddSealedModifier()
        {
            const string rawStr = @"
class C
{
    protected virtual void Method()
    {
    }
}


class D : C
{
    protected override void [|Method|]()
    {
    }
}

";

            const string expectStr = @"
class C
{
    protected virtual void Method()
    {
    }
}


class D : C
{
    protected sealed override void Method()
    {
    }
}

";

            Document document;
            TextSpan span;
            TestHelpers.TryGetDocumentAndSpanFromMarkup(rawStr, LanguageName, MetadataReferenceHelper.UsingUnityEngine, out document, out span);

            TestCodeFix(document, span, expectStr, DiagnosticDescriptors.UnsealedDerivedClass);
        }
    }
}
