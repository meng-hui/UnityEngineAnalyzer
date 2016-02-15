using Microsoft.CodeAnalysis;
using UnityEngineAnalyzer.AOT;
using UnityEngineAnalyzer.CompareTag;
using UnityEngineAnalyzer.EmptyMonoBehaviourMethods;
using UnityEngineAnalyzer.FindMethodsInUpdate;
using UnityEngineAnalyzer.OnGUI;
using UnityEngineAnalyzer.StringMethods;

namespace UnityEngineAnalyzer
{
    static class DiagnosticDescriptors
    {
        public static readonly DiagnosticDescriptor DoNotUseOnGUI = new DiagnosticDescriptor(
            id: DiagnosticIDs.DoNotUseOnGUI,
            title: new LocalizableResourceString(nameof(DoNotUseOnGUIResources.Title), DoNotUseOnGUIResources.ResourceManager, typeof(DoNotUseOnGUIResources)),
            messageFormat: new LocalizableResourceString(nameof(DoNotUseOnGUIResources.MessageFormat), DoNotUseOnGUIResources.ResourceManager, typeof(DoNotUseOnGUIResources)),
            category: DiagnosticCategories.GC, 
            defaultSeverity: DiagnosticSeverity.Warning, 
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(DoNotUseOnGUIResources.Description), DoNotUseOnGUIResources.ResourceManager, typeof(DoNotUseOnGUIResources)));

        public static readonly DiagnosticDescriptor DoNotUseStringMethods = new DiagnosticDescriptor(
            id: DiagnosticIDs.DoNotUseStringMethods,
            title: new LocalizableResourceString(nameof(DoNotUseStringMethodsResources.Title), DoNotUseStringMethodsResources.ResourceManager, typeof(DoNotUseStringMethodsResources)),
            messageFormat: new LocalizableResourceString(nameof(DoNotUseStringMethodsResources.MessageFormat), DoNotUseStringMethodsResources.ResourceManager, typeof(DoNotUseStringMethodsResources)),
            category: DiagnosticCategories.StringMethods,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(DoNotUseStringMethodsResources.Description), DoNotUseStringMethodsResources.ResourceManager, typeof(DoNotUseStringMethodsResources)));

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


        public static readonly DiagnosticDescriptor DoNotUseRemoting = new DiagnosticDescriptor(
            id: DiagnosticIDs.DoNotUseRemoting,
            title: new LocalizableResourceString(nameof(DoNotUseRemotingResources.Title), DoNotUseRemotingResources.ResourceManager, typeof(DoNotUseRemotingResources)),
            messageFormat: new LocalizableResourceString(nameof(DoNotUseRemotingResources.MessageFormat), DoNotUseRemotingResources.ResourceManager, typeof(DoNotUseRemotingResources)),
            category: DiagnosticCategories.AOT,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(DoNotUseRemotingResources.Description), DoNotUseRemotingResources.ResourceManager, typeof(DoNotUseRemotingResources)));

        public static readonly DiagnosticDescriptor DoNotUseReflectionEmit = new DiagnosticDescriptor(
            id: DiagnosticIDs.DoNotUseReflectionEmit,
            title: new LocalizableResourceString(nameof(DoNotUseReflectionEmitResources.Title), DoNotUseReflectionEmitResources.ResourceManager, typeof(DoNotUseReflectionEmitResources)),
            messageFormat: new LocalizableResourceString(nameof(DoNotUseReflectionEmitResources.MessageFormat), DoNotUseReflectionEmitResources.ResourceManager, typeof(DoNotUseReflectionEmitResources)),
            category: DiagnosticCategories.AOT,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(DoNotUseReflectionEmitResources.Description), DoNotUseReflectionEmitResources.ResourceManager, typeof(DoNotUseReflectionEmitResources)));
    }
}
