using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;
using RoslynNUnitLight;
using UnityEngineAnalyzer.AOT;

namespace UnityEngineAnalyzer.Test.AOT
{
    [TestFixture]
    sealed class DoNotUseReflectionEmitAnalyzerTests : AnalyzerTestFixture
    {
        protected override string LanguageName => LanguageNames.CSharp;
        protected override DiagnosticAnalyzer CreateAnalyzer() => new DoNotUseReflectionEmitAnalyzer();

        [Test]
        public void UsingReflectionEmit()
        {
            const string code = @"
[|using System.Reflection.Emit;|]

class C { }
";
            HasDiagnostic(code, DiagnosticIDs.DoNotUseReflectionEmit);
        }

        [Test]
        public void UsingReflectionEmitNested()
        {
            const string code = @"
namespace N
{
    [|using System.Reflection.Emit;|]
    
    class C { }
}";
            HasDiagnostic(code, DiagnosticIDs.DoNotUseReflectionEmit);
        }
    }
}
