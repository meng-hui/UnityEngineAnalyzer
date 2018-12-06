using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;
using NUnit.Core;
using NUnit.Framework;
using RoslynNUnitLight;
using UnityEngineAnalyzer.EmptyMonoBehaviourMethods;

namespace UnityEngineAnalyzer.Test.EmptyMonoBehaviourMethods
{
    [TestFixture]
    sealed class EmptyMonoBehaviourMethodsCodeFixTests:CodeFixTestFixture
    {
        protected override string LanguageName => LanguageNames.CSharp;
        protected override CodeFixProvider CreateProvider()=> new EmptyMonoBehaviourMethodsCodeFixer();

        [Test]
        public void RemoveEmptyMonoMethods()
        {
            const string rawStr = @"
using UnityEngine

class C : MonoBehaviour
{
    [|void Update() { }|]
}";

            const string expectStr = @"
using UnityEngine

class C : MonoBehaviour
{
}";

            Document document;
            TextSpan span;
            TestHelpers.TryGetDocumentAndSpanFromMarkup(rawStr, LanguageName, MetadataReferenceHelper.UsingUnityEngine, out document, out span);

            TestCodeFix(document, span, expectStr, DiagnosticDescriptors.EmptyMonoBehaviourMethod);
        }
    }
}
