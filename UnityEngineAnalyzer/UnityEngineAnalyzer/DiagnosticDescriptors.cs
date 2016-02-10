using Microsoft.CodeAnalysis;
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
    }
}
