using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace UnityEngineAnalyzer
{
    internal static class IgnoringWithCommit
    {
        private readonly static Regex IgnoreFormatRegex = new Regex(@"//\s*Ignore CA:\s*(?<CA_id>[0-9a-zA-Z]+)(,\s*(?<CA_id>[0-9a-zA-Z]+))");

        //public static bool ShouldIgnore(SyntaxNode node, string checkId)
        //{
            
        //}
    }
}
