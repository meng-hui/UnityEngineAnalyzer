namespace UnityEngineAnalyzer.CLI.Reporting
{
    public enum DiagnosticInfoSeverity
    {
        Hidden = 0,
        Info = 1,
        Warning = 2,
        Error = 3
    }

    public class DiagnosticInfo
    {
        public string Id { get; set; }
        public string Message { get; set; }
        public string FileName { get; set; }
        public int LineNumber { get; set; }
        public DiagnosticInfoSeverity Severity { get; set; }
    }
}