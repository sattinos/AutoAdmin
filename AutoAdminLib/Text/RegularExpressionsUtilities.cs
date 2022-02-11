using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AutoAdminLib.Text
{
    public static class RegularExpressionsUtilities
    {
        public static List<string> GetAllCaptures(string pattern, string input, string captureName)
        {
            if (
                string.IsNullOrWhiteSpace(pattern) ||
                string.IsNullOrWhiteSpace(input) ||
                string.IsNullOrWhiteSpace(captureName)
                )
            {
                return new List<string>();
            }
            var names = new List<string>();
            var matches = Regex.Matches(input, pattern, RegexOptions.IgnoreCase);
            foreach (Match match in matches)
            {
                foreach (Capture capture in match.Groups[captureName].Captures)
                {
                    names.Add(capture.Value);
                }
            }
            return names;
        }
    }
}