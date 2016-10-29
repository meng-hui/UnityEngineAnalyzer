using Microsoft.CodeAnalysis;
using UnityEngineAnalyzer.AOT;
using UnityEngineAnalyzer.CompareTag;
using UnityEngineAnalyzer.Coroutines;
using UnityEngineAnalyzer.EmptyMonoBehaviourMethods;
using UnityEngineAnalyzer.FindMethodsInUpdate;
using UnityEngineAnalyzer.ForEachInUpdate;
using UnityEngineAnalyzer.OnGUI;
using UnityEngineAnalyzer.StringMethods;

namespace UnityEngineAnalyzer
{
    static class DiagnosticDescriptors
    {
        //NOTES: Naming of Descriptors are a bit inconsistant

        public static readonly DiagnosticDescriptor DoNotUseOnGUI = new DiagnosticDescriptor(
            id: DiagnosticIDs.DoNotUseOnGUI,
            title: new LocalizableResourceString(nameof(DoNotUseOnGUIResources.Title), DoNotUseOnGUIResources.ResourceManager, typeof(DoNotUseOnGUIResources)),
            messageFormat: new LocalizableResourceString(nameof(DoNotUseOnGUIResources.MessageFormat), DoNotUseOnGUIResources.ResourceManager, typeof(DoNotUseOnGUIResources)),
            category: DiagnosticCategories.GC,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(DoNotUseOnGUIResources.Description), DoNotUseOnGUIResources.ResourceManager, typeof(DoNotUseOnGUIResources)));

        public static readonly DiagnosticDescriptor DoNotUseStringMethods = new DiagnosticDescriptor(
            id: DiagnosticIDs.DoNotUseStringMethods,
            title: new LocalizableResourceString(nameof(DoNotUseStringMethodsResources.Title), DoNotUseStringMethodsResources.ResourceManager, typeof(DoNotUseStringMethodsResources)),
            messageFormat: new LocalizableResourceString(nameof(DoNotUseStringMethodsResources.MessageFormat), DoNotUseStringMethodsResources.ResourceManager, typeof(DoNotUseStringMethodsResources)),
            category: DiagnosticCategories.StringMethods,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(DoNotUseStringMethodsResources.Description), DoNotUseStringMethodsResources.ResourceManager, typeof(DoNotUseStringMethodsResources)));

        public static readonly DiagnosticDescriptor DoNotUseCoroutines = new DiagnosticDescriptor(
            id: DiagnosticIDs.DoNotUseCoroutines,
            title: new LocalizableResourceString(nameof(DoNotUseCoroutinesResources.Title), DoNotUseCoroutinesResources.ResourceManager, typeof(DoNotUseCoroutinesResources)),
            messageFormat: new LocalizableResourceString(nameof(DoNotUseCoroutinesResources.MessageFormat), DoNotUseCoroutinesResources.ResourceManager, typeof(DoNotUseCoroutinesResources)),
            category: DiagnosticCategories.GC,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(DoNotUseCoroutinesResources.Description), DoNotUseCoroutinesResources.ResourceManager, typeof(DoNotUseCoroutinesResources)));


        public static readonly DiagnosticDescriptor EmptyMonoBehaviourMethod = new DiagnosticDescriptor(
            id: DiagnosticIDs.EmptyMonoBehaviourMethod,
            title: new LocalizableResourceString(nameof(EmptyMonoBehaviourMethodsResources.Title), EmptyMonoBehaviourMethodsResources.ResourceManager, typeof(EmptyMonoBehaviourMethodsResources)),
            messageFormat: new LocalizableResourceString(nameof(EmptyMonoBehaviourMethodsResources.MessageFormat), EmptyMonoBehaviourMethodsResources.ResourceManager, typeof(EmptyMonoBehaviourMethodsResources)),
            category: DiagnosticCategories.Miscellaneous,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(EmptyMonoBehaviourMethodsResources.Description), EmptyMonoBehaviourMethodsResources.ResourceManager, typeof(EmptyMonoBehaviourMethodsResources)));

        public static readonly DiagnosticDescriptor UseCompareTag = new DiagnosticDescriptor(
            id: DiagnosticIDs.UseCompareTag,
            title: new LocalizableResourceString(nameof(UseCompareTagResources.Title), UseCompareTagResources.ResourceManager, typeof(UseCompareTagResources)),
            messageFormat: new LocalizableResourceString(nameof(UseCompareTagResources.MessageFormat), UseCompareTagResources.ResourceManager, typeof(UseCompareTagResources)),
            category: DiagnosticCategories.GC,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(UseCompareTagResources.Description), UseCompareTagResources.ResourceManager, typeof(UseCompareTagResources)));

        public static readonly DiagnosticDescriptor DoNotUseFindMethodsInUpdate = new DiagnosticDescriptor(
            id: DiagnosticIDs.DoNotUseFindMethodsInUpdate,
            title: new LocalizableResourceString(nameof(DoNotUseFindMethodsInUpdateResources.Title), DoNotUseFindMethodsInUpdateResources.ResourceManager, typeof(DoNotUseFindMethodsInUpdateResources)),
            messageFormat: new LocalizableResourceString(nameof(DoNotUseFindMethodsInUpdateResources.MessageFormat), DoNotUseFindMethodsInUpdateResources.ResourceManager, typeof(DoNotUseFindMethodsInUpdateResources)),
            category: DiagnosticCategories.Performance,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(DoNotUseFindMethodsInUpdateResources.Description), DoNotUseFindMethodsInUpdateResources.ResourceManager, typeof(DoNotUseFindMethodsInUpdateResources)));

        public static readonly DiagnosticDescriptor DoNotUseFindMethodsInUpdateRecursive = new DiagnosticDescriptor(
            id: DiagnosticIDs.DoNotUseFindMethodsInUpdate,
            title: new LocalizableResourceString(nameof(DoNotUseFindMethodsInUpdateResources.Title), DoNotUseFindMethodsInUpdateResources.ResourceManager, typeof(DoNotUseFindMethodsInUpdateResources)),
            messageFormat: new LocalizableResourceString(nameof(DoNotUseFindMethodsInUpdateResources.MessageFormatRecursive), DoNotUseFindMethodsInUpdateResources.ResourceManager, typeof(DoNotUseFindMethodsInUpdateResources)),
            category: DiagnosticCategories.Performance,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(DoNotUseFindMethodsInUpdateResources.Description), DoNotUseFindMethodsInUpdateResources.ResourceManager, typeof(DoNotUseFindMethodsInUpdateResources)));

        public static readonly DiagnosticDescriptor DoNotUseRemoting = new DiagnosticDescriptor(
            id: DiagnosticIDs.DoNotUseRemoting,
            title: new LocalizableResourceString(nameof(DoNotUseRemotingResources.Title), DoNotUseRemotingResources.ResourceManager, typeof(DoNotUseRemotingResources)),
            messageFormat: new LocalizableResourceString(nameof(DoNotUseRemotingResources.MessageFormat), DoNotUseRemotingResources.ResourceManager, typeof(DoNotUseRemotingResources)),
            category: DiagnosticCategories.AOT,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(DoNotUseRemotingResources.Description), DoNotUseRemotingResources.ResourceManager, typeof(DoNotUseRemotingResources)));

        public static readonly DiagnosticDescriptor DoNotUseReflectionEmit = new DiagnosticDescriptor(
            id: DiagnosticIDs.DoNotUseReflectionEmit,
            title: new LocalizableResourceString(nameof(DoNotUseReflectionEmitResources.Title), DoNotUseReflectionEmitResources.ResourceManager, typeof(DoNotUseReflectionEmitResources)),
            messageFormat: new LocalizableResourceString(nameof(DoNotUseReflectionEmitResources.MessageFormat), DoNotUseReflectionEmitResources.ResourceManager, typeof(DoNotUseReflectionEmitResources)),
            category: DiagnosticCategories.AOT,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(DoNotUseReflectionEmitResources.Description), DoNotUseReflectionEmitResources.ResourceManager, typeof(DoNotUseReflectionEmitResources)));

        public static readonly DiagnosticDescriptor TypeGetType = new DiagnosticDescriptor(
            id: DiagnosticIDs.TypeGetType,
            title: new LocalizableResourceString(nameof(TypeGetTypeResources.Title), TypeGetTypeResources.ResourceManager, typeof(TypeGetTypeResources)),
            messageFormat: new LocalizableResourceString(nameof(TypeGetTypeResources.MessageFormat), TypeGetTypeResources.ResourceManager, typeof(TypeGetTypeResources)),
            category: DiagnosticCategories.AOT,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(TypeGetTypeResources.Description), TypeGetTypeResources.ResourceManager, typeof(TypeGetTypeResources)));

        public static readonly DiagnosticDescriptor DoNotUseForEachInUpdate  = new DiagnosticDescriptor(
            id: DiagnosticIDs.DoNotUseForEachInUpdate,
            title: new LocalizableResourceString(nameof(DoNotUseForEachInUpdateResources.Title), DoNotUseForEachInUpdateResources.ResourceManager, typeof(DoNotUseForEachInUpdateResources)),
            messageFormat: new LocalizableResourceString(nameof(DoNotUseForEachInUpdateResources.MessageFormat), DoNotUseForEachInUpdateResources.ResourceManager, typeof(DoNotUseForEachInUpdateResources)),
            category: DiagnosticCategories.Performance,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(DoNotUseForEachInUpdateResources.Description), DoNotUseForEachInUpdateResources.ResourceManager, typeof(DoNotUseForEachInUpdateResources))
        );
    }
}
