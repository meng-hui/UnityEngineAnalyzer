namespace UnityEngineAnalyzer
{
    public class UnityVersionSpan
    {
        public UnityVersion First { get; set; }
        public UnityVersion Last { get; set; }

        public UnityVersionSpan (UnityVersion first, UnityVersion last)
        {
            First = first;
            Last = last;
        }
    }

    public enum UnityVersion
    {
        NONE,
        UNITY_1_0,
        UNITY_2_0,
        UNITY_3_0,
        UNITY_3_5,
        UNITY_4_0,
        UNITY_4_1,
        UNITY_4_2,
        UNITY_4_3,
        UNITY_4_4,
        UNITY_4_5,
        UNITY_4_6,
        UNITY_4_7,
        UNITY_5_0,
        UNITY_5_1,
        UNITY_5_2,
        UNITY_5_3,
        UNITY_5_4,
        UNITY_5_5,
        UNITY_5_6,
        UNITY_2017_0,
        UNITY_2017_1,
        UNITY_2017_2,
        UNITY_2017_3,
        LATEST,
    }
}