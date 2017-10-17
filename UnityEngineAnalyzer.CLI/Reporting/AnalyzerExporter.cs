using Microsoft.CodeAnalysis;
using System;
using System.IO;
using static UnityEngineAnalyzer.CLI.Reporting.DiagnosticInfo;

namespace UnityEngineAnalyzer.CLI.Reporting
{
    public abstract class AnalyzerExporter : IAnalyzerExporter
    {
        protected DiagnosticInfoSeverity MinimalSeverity;
        private readonly UnityVersion unityVersion;

        public AnalyzerExporter(DiagnosticInfoSeverity MinimalSeverity, UnityVersion unityVersion)
        {
            this.MinimalSeverity = MinimalSeverity;
            this.unityVersion = unityVersion;
        }

        public bool AbleToAnalyzer(DiagnosticInfoSeverity currentSeverity, DiagnosticDescriptor diagnosticInfo, UnityVersion unityVersion = UnityVersion.ALL)
        {
            if (MinimalSeverity < currentSeverity)
            {
                return false;
            }

            var off = DiagnosticDescriptors.GetVersion(diagnosticInfo);

            if (unityVersion < off.Item1 || unityVersion > off.Item2)
            {
                return false;
            }

            return true;
        }

        public abstract void AppendDiagnostic(DiagnosticInfo diagnosticInfo);
        public abstract void FinalizeExporter(TimeSpan duration);
        public abstract void InitializeExporter(FileInfo projectFile);
        public abstract void NotifyException(Exception exception);
    }
}
