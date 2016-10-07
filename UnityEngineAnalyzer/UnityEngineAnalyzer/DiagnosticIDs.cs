
namespace UnityEngineAnalyzer
{
    public static class DiagnosticIDs
    {
        public const string EmptyMonoBehaviourMethod = "UEA0003";
        public const string UseCompareTag = "UEA0004";
        public const string DoNotUseFindMethodsInUpdate = "UEA0005";

        //NOTES: These should probably be on their own analyzer - as they are not specific to Unity
        public const string DoNotUseRemoting = "AOT0001";
        public const string DoNotUseReflectionEmit = "AOT0002";
        public const string TypeGetType = "AOT0003";
    }
}
