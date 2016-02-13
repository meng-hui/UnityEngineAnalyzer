using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;
using RoslynNUnitLight;
using UnityEngineAnalyzer.AOT;

namespace UnityEngineAnalyzer.Test.AOT
{
    [TestFixture]
    sealed class DoNotUseRemotingAnalyzerTests : AnalyzerTestFixture
    {
        protected override string LanguageName => LanguageNames.CSharp;
        protected override DiagnosticAnalyzer CreateAnalyzer() => new DoNotUseRemotingAnalyzer();

        [Test]
        public void UsingRemoting()
        {
            const string code = @"
[|using System.Runtime.Remoting;|]

class C { }
";
            HasDiagnostic(code, DiagnosticIDs.DoNotUseRemoting);
        }

        [Test]
        public void UsingRemotingNested()
        {
            const string code = @"
namespace N
{
    [|using System.Runtime.Remoting;|]
    
    class C { }
}";
            HasDiagnostic(code, DiagnosticIDs.DoNotUseRemoting);
        }
    }
}
