using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.IO;

namespace UnityEngineAnalyzer.Test
{
    static class MetadataReferenceHelper
    {
        private static readonly DirectoryInfo ExecutingLocation = Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location);
        private static readonly DirectoryInfo SolutionDirectory = ExecutingLocation.Parent.Parent.Parent.Parent;
        private static readonly string UnityEnginePath = Path.Combine(SolutionDirectory.FullName, "SolutionItems", "UnityEngine.dll");

        public static readonly ImmutableList<MetadataReference> UsingUnityEngine = 
            ImmutableList.Create(MetadataReference.CreateFromFile(UnityEnginePath) as MetadataReference);
    }
}
