using Microsoft.CodeAnalysis;
using UnityEngineAnalyzer.OnGUI;

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
    }
}
