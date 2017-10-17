using Microsoft.CodeAnalysis;
using System;
using System.Linq;
using System.Resources;
using UnityEngineAnalyzer.Animator;
using UnityEngineAnalyzer.AOT;
using UnityEngineAnalyzer.Camera;
using UnityEngineAnalyzer.CompareTag;
using UnityEngineAnalyzer.Coroutines;
using UnityEngineAnalyzer.EmptyMonoBehaviourMethods;
using UnityEngineAnalyzer.FindMethodsInUpdate;
using UnityEngineAnalyzer.ForEachInUpdate;
using UnityEngineAnalyzer.IL2CPP;
using UnityEngineAnalyzer.Material;
using UnityEngineAnalyzer.OnGUI;
using UnityEngineAnalyzer.Physics;
using UnityEngineAnalyzer.StringMethods;

namespace UnityEngineAnalyzer
{
    public static class DiagnosticDescriptors
    {
        public static readonly DiagnosticDescriptor DoNotUseOnGUI;
        public static readonly DiagnosticDescriptor DoNotUseStringMethods;
        public static readonly DiagnosticDescriptor DoNotUseCoroutines;
        public static readonly DiagnosticDescriptor EmptyMonoBehaviourMethod;
        public static readonly DiagnosticDescriptor UseCompareTag;
        public static readonly DiagnosticDescriptor DoNotUseFindMethodsInUpdate;
        public static readonly DiagnosticDescriptor DoNotUseFindMethodsInUpdateRecursive;
        public static readonly DiagnosticDescriptor DoNotUseRemoting;
        public static readonly DiagnosticDescriptor DoNotUseReflectionEmit;
        public static readonly DiagnosticDescriptor TypeGetType;
        public static readonly DiagnosticDescriptor DoNotUseForEachInUpdate;
        public static readonly DiagnosticDescriptor UnsealedDerivedClass;
        public static readonly DiagnosticDescriptor InvokeFunctionMissing;
        public static readonly DiagnosticDescriptor DoNotUseStateName;
        public static readonly DiagnosticDescriptor DoNotUseStringPropertyNames;
        public static readonly DiagnosticDescriptor UseNonAllocMethods;
        public static readonly DiagnosticDescriptor CameraMainIsSlow;

        static DiagnosticDescriptors()
        {
            DoNotUseOnGUI = CreateDiagnosticDescriptor<DoNotUseOnGUIResources>(DiagnosticIDs.DoNotUseOnGUI, DiagnosticCategories.GC, DiagnosticSeverity.Info);
            DoNotUseStringMethods = CreateDiagnosticDescriptor<DoNotUseStringMethodsResources>(DiagnosticIDs.DoNotUseStringMethods, DiagnosticCategories.GC, DiagnosticSeverity.Info);
            DoNotUseCoroutines = CreateDiagnosticDescriptor<DoNotUseCoroutinesResources>(DiagnosticIDs.DoNotUseCoroutines, DiagnosticCategories.GC, DiagnosticSeverity.Info);
            EmptyMonoBehaviourMethod = CreateDiagnosticDescriptor<EmptyMonoBehaviourMethodsResources>(DiagnosticIDs.EmptyMonoBehaviourMethod, DiagnosticCategories.Miscellaneous, DiagnosticSeverity.Warning);
            UseCompareTag = CreateDiagnosticDescriptor<UseCompareTagResources>(DiagnosticIDs.UseCompareTag, DiagnosticCategories.GC, DiagnosticSeverity.Warning);
            DoNotUseFindMethodsInUpdate = CreateDiagnosticDescriptor<DoNotUseFindMethodsInUpdateResources>(DiagnosticIDs.DoNotUseFindMethodsInUpdate, DiagnosticCategories.Performance, DiagnosticSeverity.Warning);
            DoNotUseFindMethodsInUpdateRecursive = CreateDiagnosticDescriptor<DoNotUseFindMethodsInUpdateResources>(DiagnosticIDs.DoNotUseFindMethodsInUpdate, DiagnosticCategories.Performance, DiagnosticSeverity.Warning);
            DoNotUseRemoting = CreateDiagnosticDescriptor<DoNotUseRemotingResources>(DiagnosticIDs.DoNotUseRemoting, DiagnosticCategories.AOT, DiagnosticSeverity.Info);
            DoNotUseReflectionEmit = CreateDiagnosticDescriptor<DoNotUseReflectionEmitResources>(DiagnosticIDs.DoNotUseReflectionEmit, DiagnosticCategories.AOT, DiagnosticSeverity.Info);
            TypeGetType = CreateDiagnosticDescriptor<TypeGetTypeResources>(DiagnosticIDs.TypeGetType, DiagnosticCategories.AOT, DiagnosticSeverity.Info);
            DoNotUseForEachInUpdate = CreateDiagnosticDescriptor<DoNotUseForEachInUpdateResources>(DiagnosticIDs.DoNotUseForEachInUpdate, DiagnosticCategories.Performance, DiagnosticSeverity.Warning);
            UnsealedDerivedClass = CreateDiagnosticDescriptor<UnsealedDerivedClassResources>(DiagnosticIDs.UnsealedDerivedClass, DiagnosticCategories.Performance, DiagnosticSeverity.Warning);
            InvokeFunctionMissing = CreateDiagnosticDescriptor<InvokeFunctionMissingResources>(DiagnosticIDs.InvokeFunctionMissing, DiagnosticCategories.Performance, DiagnosticSeverity.Warning);
            DoNotUseStateName = CreateDiagnosticDescriptor<DoNotUseStateNameResource>(DiagnosticIDs.DoNotUseStateNameInAnimator, DiagnosticCategories.Performance, DiagnosticSeverity.Warning);
            DoNotUseStringPropertyNames = CreateDiagnosticDescriptor<DoNotUseStringPropertyNamesResource>(DiagnosticIDs.DoNotUseStringPropertyNamesInMaterial, DiagnosticCategories.Performance, DiagnosticSeverity.Warning);
            UseNonAllocMethods = CreateDiagnosticDescriptor<UseNonAllocMethodsResources>(DiagnosticIDs.PhysicsUseNonAllocMethods, DiagnosticCategories.GC, DiagnosticSeverity.Warning);
            CameraMainIsSlow = CreateDiagnosticDescriptor<CameraMainResource>(DiagnosticIDs.CameraMainIsSlow, DiagnosticCategories.GC, DiagnosticSeverity.Warning);
        }

        private static DiagnosticDescriptor CreateDiagnosticDescriptor<T>(string id, string category, DiagnosticSeverity severity, UnityVersion latest = UnityVersion.ALL, bool isEnabledByDefault = true)
        {
            var resourceManager = new ResourceManager(typeof(T));

            return new DiagnosticDescriptor(
            id: id,
            title: new LocalizableResourceString("Title", resourceManager, typeof(T)),
            messageFormat: new LocalizableResourceString("MessageFormat", resourceManager, typeof(T)),
            category: category,
            defaultSeverity: severity,
            isEnabledByDefault: isEnabledByDefault,
            customTags: CreateUnityVersionInfo(latest, latest),
            description: new LocalizableResourceString("Description", resourceManager, typeof(T)));
        }

        private static string[] CreateUnityVersionInfo(UnityVersion start, UnityVersion end)
        {
            return new string[] { Enum.GetName(typeof(UnityVersion), start), Enum.GetName(typeof(UnityVersion), end) };
        }

        //TODO USE: (UnityVersion start, UnityVersion end) Tuple when updated to Net Standard 2.0
        public static Tuple<UnityVersion, UnityVersion> GetVersion(DiagnosticDescriptor dc)
        {
            var list = dc.CustomTags.ToList();
            var start = (UnityVersion)Enum.Parse(typeof(UnityVersion), list[0]);
            var end = (UnityVersion)Enum.Parse(typeof(UnityVersion), list[1]);

            return Tuple.Create(start, end);
        }
    }
}
