using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace UnityEngineAnalyzer.CLI
{
    //NOTE: This class would benefit from UnitTests specially: TryParseUnityVersion
    internal class UnityVersionResolver
    {
        private const UnityVersion DEFAULT_UNITY_VERSION = UnityVersion.LATEST;

        public UnityVersion ResolveVersion(Options options)
        {
            if (options.Version != UnityVersion.NONE)
            {
                return options.Version;
            }

            //THIS ONLY WORKS ON UNITY >= 5, before that ProjectVersion.txt did not exists
            if (ResolveProjectVersionFilePath(options) != null)
            {
                var projectVersionString = File.ReadAllText(ResolveProjectVersionFilePath(options));
                return TryParseUnityVersion(projectVersionString);
            }

            return DEFAULT_UNITY_VERSION;
        }

        private string ResolveProjectVersionFilePath(Options options)
        {
            var projectPath = new FileInfo(options.ProjectFile).Directory;
            var path = Path.Combine(projectPath.FullName, "ProjectSettings", "ProjectVersion.txt");
            var projectVersionFile = new FileInfo(path);

            if (projectVersionFile.Exists)
            {
                return projectVersionFile.FullName;
            }

            return null;
        }

        private UnityVersion TryParseUnityVersion(string version)
        {
            string editorText = "m_EditorVersion: ";
            var match = Regex.Match(version, editorText + "[0-9.a-z]*");

            string src = match.Value.Substring(editorText.Length);
            src = src.Replace('.', '_');
            src = src.Substring(0, src.IndexOf('_') + 2);

            var unityVersions = Enum.GetValues(typeof(UnityVersion)).Cast<UnityVersion>();
            foreach (var unityVersion in unityVersions)
            {
                if (Enum.GetName(typeof(UnityVersion), unityVersion).Contains(src))
                {
                    return unityVersion;
                }
            }

            return DEFAULT_UNITY_VERSION;
        }
    }
}
