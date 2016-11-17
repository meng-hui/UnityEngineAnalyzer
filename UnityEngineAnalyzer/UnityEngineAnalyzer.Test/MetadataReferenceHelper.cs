using System;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.IO;
using System.Linq;


namespace UnityEngineAnalyzer.Test
{
    static class MetadataReferenceHelper
    {
        public static readonly ImmutableList<MetadataReference> UsingUnityEngine =
            ImmutableList.Create(GetUnityMetadataReference(), GetSystem(), GetSystemCore());

        private static MetadataReference GetUnityMetadataReference()
        {
            var unityEnginePath = Path.Combine(Environment.ExpandEnvironmentVariables("%ProgramW6432%"), @"Unity\Editor\Data\Managed", "UnityEngine.dll");

            return MetadataReference.CreateFromFile(unityEnginePath);
        }

        private static MetadataReference GetSystem()
        {
            var assemblyPath = typeof(object).Assembly.Location;
            return MetadataReference.CreateFromFile(assemblyPath);
        }

        private static MetadataReference GetSystemCore()
        {
            var assemblyPath = typeof(Enumerable).Assembly.Location;
            return MetadataReference.CreateFromFile(assemblyPath);
        }
    }
}
