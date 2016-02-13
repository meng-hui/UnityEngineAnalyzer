using NUnit.Framework;
using RoslynNUnitLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
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
