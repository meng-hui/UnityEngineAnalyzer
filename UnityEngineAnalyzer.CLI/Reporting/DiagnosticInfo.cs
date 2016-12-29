namespace UnityEngineAnalyzer.CLI.Reporting
{


    public class DiagnosticInfo
    {
        //TODO: Rename this to something like AnalysisResult

        public string Id { get; set; }
        public string Message { get; set; }
        public string FileName { get; set; }
        public int LineNumber { get; set; }
        public int CharacterPosition { get; set; }
        public DiagnosticInfoSeverity Severity { get; set; }

        public enum DiagnosticInfoSeverity
        {
            Hidden = 0,
            Info = 1,
            Warning = 2,
            Error = 3
        }
    }
}