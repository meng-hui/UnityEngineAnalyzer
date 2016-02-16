
namespace UnityEngineAnalyzer
{
    public static class DiagnosticIDs
    {
        public const string DoNotUseOnGUI = "UEA0001";
        public const string DoNotUseStringMethods = "UEA0002";
        public const string EmptyMonoBehaviourMethod = "UEA0003";
        public const string UseCompareTag = "UEA0004";
        public const string DoNotUseFindMethodsInUpdate = "UEA0005";
        public const string DoNotUseCoroutines = "UEA0006";

        public const string DoNotUseRemoting = "AOT0001";
        public const string DoNotUseReflectionEmit = "AOT0002";
        public const string TypeGetType = "AOT0003";
    }
}
