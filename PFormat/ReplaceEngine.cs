using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PFormat
{
    internal static class ReplaceEngine
    {
        #region Internal Methods

        internal static string FixLinefeedCode(string value)
        {
            return Regex.Replace(value, "\\n(?<=[^\\r])", Environment.NewLine);
        }

        internal static string Replace(string format, Dictionary<string, string> fields)
        {
            string result = format;

            foreach (KeyValuePair<string, string> field in fields)
            {
                string key = field.Key;

                if (string.IsNullOrEmpty(key)) continue;

                result = result.Replace($"${{{key}}}", field.Value);
            }

            return result;
        }

        #endregion
    }
}
