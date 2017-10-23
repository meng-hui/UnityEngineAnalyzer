﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using RoslynNUnitLight;
using UnityEngineAnalyzer.StringMethods;

namespace UnityEngineAnalyzer.Test.StringMethods
{
    [TestFixture]
    sealed class DoNotUseStringMethodsAnalyzerTests : AnalyzerTestFixture
    {
        protected override string LanguageName => LanguageNames.CSharp;
        protected override DiagnosticAnalyzer CreateAnalyzer() => new DoNotUseStringMethodsAnalyzer();

        [Test]
        public void SendMessageUsedInMonoBehaviourClass()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    void Start() { [|SendMessage(string.Empty)|]; }
}";

            Document document;
            TextSpan span;
            TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine, out document, out span);

            HasDiagnostic(document, span, DiagnosticIDs.DoNotUseStringMethods);
        }

        [Test]
        public void SendMessageUpwardsUsedInMonoBehaviourClass()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    void Start() { [|SendMessageUpwards(string.Empty)|]; }
}";

            Document document;
            TextSpan span;
            TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine, out document, out span);

            HasDiagnostic(document, span, DiagnosticIDs.DoNotUseStringMethods);
        }

        [Test]
        public void BroadcastMessageUsedInMonoBehaviourClass()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    void Start() { [|BroadcastMessage(string.Empty)|]; }
}";

            Document document;
            TextSpan span;
            TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine, out document, out span);

            HasDiagnostic(document, span, DiagnosticIDs.DoNotUseStringMethods);
        }

        [Test]
        public void SendMessageUsedByGameObject()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    private GameObject go;
    void Start() { [|go.SendMessage(string.Empty)|]; }
}";

            Document document;
            TextSpan span;
            TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine, out document, out span);

            HasDiagnostic(document, span, DiagnosticIDs.DoNotUseStringMethods);
        }

        [Test]
        public void SendMessageUpwardsUsedByGameObject()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    private GameObject go;
    void Start() { [|go.SendMessageUpwards(string.Empty)|]; }
}";

            Document document;
            TextSpan span;
            TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine, out document, out span);

            HasDiagnostic(document, span, DiagnosticIDs.DoNotUseStringMethods);
        }

        [Test]
        public void BroadcastMessageUsedByGameObject()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    private GameObject go;
    void Start() { [|go.BroadcastMessage(string.Empty)|]; }
}";

            Document document;
            TextSpan span;
            TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine, out document, out span);

            HasDiagnostic(document, span, DiagnosticIDs.DoNotUseStringMethods);
        }

        [Test]
        public void SendMessageUsedByMonoBehaviourClass()
        {
            const string code = @"
using UnityEngine;

class CC : MonoBehaviour { }

class C : MonoBehaviour
{
    private CC cc;
    void Start() { [|cc.SendMessage(string.Empty)|]; }
}";

            Document document;
            TextSpan span;
            TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine, out document, out span);

            HasDiagnostic(document, span, DiagnosticIDs.DoNotUseStringMethods);
        }

        [Test]
        public void SendMessageUpwardsUsedByMonoBehaviourClass()
        {
            const string code = @"
using UnityEngine;

class CC : MonoBehaviour { }

class C : MonoBehaviour
{
    private CC cc;
    void Start() { [|cc.SendMessageUpwards(string.Empty)|]; }
}";

            Document document;
            TextSpan span;
            TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine, out document, out span);

            HasDiagnostic(document, span, DiagnosticIDs.DoNotUseStringMethods);
        }

        [Test]
        public void BroadcastMessageUsedByMonoBehaviourClass()
        {
            const string code = @"
using UnityEngine;

class CC : MonoBehaviour { }

class C : MonoBehaviour
{
    private CC cc;
    void Start() { [|cc.BroadcastMessage(string.Empty)|]; }
}";

            Document document;
            TextSpan span;
            TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine, out document, out span);

            HasDiagnostic(document, span, DiagnosticIDs.DoNotUseStringMethods);
        }
    }
}
