using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using RoslynNUnitLight;
using UnityEngineAnalyzer.Language;
using UnityEngineAnalyzer.LogicError;


//using Microsoft.CodeAnalysis.Workspaces;

namespace UnityEngineAnalyzer.Test.LogicError
{
    [TestFixture]
    sealed class InfiniteRecursiveCallTest : AnalyzerTestFixture
    {

        protected override string LanguageName => LanguageNames.CSharp;
        protected override DiagnosticAnalyzer CreateAnalyzer() => new InfiniteRecursiveCallAnalyzer();

        [Test]
        public void InfiniteRecursiveCall1()
        {
            var code = @"
class A
{
    void M1()
    {
        [|M1()|];
    }
}";

            Document document;
            TextSpan span;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine,
                out document, out span))
            {
                HasDiagnostic(document, span, DiagnosticIDs.InfiniteRecursiveCall);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }


        [Test]
        public void InfiniteRecursiveCall2()
        {
            var code = @"
class A
{
    void M1()
    {
        [|M2()|];
    }
    void M2()
    {
        M1();
    }
}";

            Document document;
            TextSpan span;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine,
                out document, out span))
            {
                HasDiagnostic(document, span, DiagnosticIDs.InfiniteRecursiveCall);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }



        [Test]
        public void InfiniteRecursiveCall3()
        {
            var code = @"
class A
{
    void M1()
    {
        [|M2()|];
    }
    void M2()
    {
        M3();
    }
    void M3()
    {
        M1();
    }
}";

            Document document;
            TextSpan span;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine,
                out document, out span))
            {
                HasDiagnostic(document, span, DiagnosticIDs.InfiniteRecursiveCall);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }


        [Test]
        public void InfiniteRecursiveCallDouble()
        {
            var code = @"
class A
{
    void M1()
    {
        [|M2()|];
    }
    void M2()
    {
        M1();
    }
    void M3()
    {
        [|M3()|];
    }

}";

            Document document;
            TextSpan span;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine,
                out document, out span))
            {
                HasDiagnostic(document, span, DiagnosticIDs.InfiniteRecursiveCall);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }

        [Test]
        public void InfiniteRecursiveCallWithBranch1()
        {
            var code = @"
class A
{
    private bool m_bool;
    void M1()
    {
        if(m_bool)
        {
            [|M1()|];
        }
    }
}";

            Document document;
            TextSpan span;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine,
                out document, out span))
            {
                NoDiagnostic(document, DiagnosticIDs.InfiniteRecursiveCall);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }



        [Test]
        public void InfiniteRecursiveCallWithBranch2()
        {
            var code = @"
class A
{
    private bool m_bool;
    void M1()
    {
        switch(m_bool)
        {
            default:
                [|M1()|];
                break;
        }
    }
}";

            Document document;
            TextSpan span;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine,
                out document, out span))
            {
                NoDiagnostic(document, DiagnosticIDs.InfiniteRecursiveCall);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }


        [Test]
        public void InfiniteRecursiveCallWithBranch3()
        {
            var code = @"
class A
{
    private bool m_bool;
    void M1()
    {
        switch(m_bool)
        {
            default:
                [|M1()|];
                break;
        }
    }
}";

            Document document;
            TextSpan span;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine,
                out document, out span))
            {
                NoDiagnostic(document, DiagnosticIDs.InfiniteRecursiveCall);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }

        [Test]
        public void InfiniteRecursiveCallWithBranch4()
        {
            var code = @"
class A
{
    private bool m_bool;
    void M1()
    {
        int[] a = new int[3];
        foreach(var oneInt in a)
        {
            [|M1()|];
        }
    }
}";

            Document document;
            TextSpan span;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine,
                out document, out span))
            {
                NoDiagnostic(document, DiagnosticIDs.InfiniteRecursiveCall);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }

        [Test]
        public void InfiniteRecursiveCallWithBranch5()
        {
            var code = @"
class A
{
    private bool m_bool;
    void M1()
    {
        for(int i = 0; i < 3; ++i)
        {
            [|M1()|];
        }
    }
}";

            Document document;
            TextSpan span;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine,
                out document, out span))
            {
                NoDiagnostic(document, DiagnosticIDs.InfiniteRecursiveCall);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }

        [Test]
        public void InfiniteRecursiveCallWithBranch6()
        {
            var code = @"
class A
{
    private bool m_bool;
    void M1()
    {
        while(true)
        {
            [|M1()|];
        }
    }
}";

            Document document;
            TextSpan span;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine,
                out document, out span))
            {
                NoDiagnostic(document, DiagnosticIDs.InfiniteRecursiveCall);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }

        [Test]
        public void InfiniteRecursiveCallWithBranch7()
        {
            var code = @"
class A
{
    private bool m_bool;
    void M1()
    {
        return;
        [|M1()|];
    }
}";

            Document document;
            TextSpan span;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine,
                out document, out span))
            {
                NoDiagnostic(document, DiagnosticIDs.InfiniteRecursiveCall);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }


        [Test]
        public void NotInfiniteRecursiveCall()
        {
            var code = @"
class A
{
    private bool m_bool;
    void M1()
    {
        [|M2()|];
    }
    
    void M2() {}
}";

            Document document;
            TextSpan span;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine,
                out document, out span))
            {
                NoDiagnostic(document, DiagnosticIDs.InfiniteRecursiveCall);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }
    }
}
