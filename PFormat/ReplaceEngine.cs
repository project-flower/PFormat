using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PFormat
{
    internal static class ReplaceEngine
    {
        #region Private Fields

        private static Dictionary<string, string> fields;

        #endregion

        #region Internal Methods

        internal static string FixLinefeedCode(string value)
        {
            return Regex.Replace(value, "\\n(?<=[^\\r])", Environment.NewLine);
        }

        internal static string Replace(string format, Dictionary<string, string> fields)
        {
            ReplaceEngine.fields = fields;
            return Regex.Replace(format, "\\$[{]([{](?<options>[^}]+)[}])*(?<name>[^}]+)[}]", Replace);
        }

        #endregion

        #region Private Methods

        private static bool IsSameAs(this string text1, string text2)
        {
            return (string.Compare(text1, text2, true) == 0);
        }

        private static object Parse(string value, string type)
        {
            if (type.IsSameAs("bool") || type.IsSameAs("boolean")) return bool.Parse(value);
            else if (type.IsSameAs("byte")) return byte.Parse(value);
            else if (type.IsSameAs("char")) return char.Parse(value);
            else if (type.IsSameAs("datetime")) return DateTime.Parse(value);
            else if (type.IsSameAs("decimal")) return decimal.Parse(value);
            else if (type.IsSameAs("double")) return double.Parse(value);
            else if (type.IsSameAs("float") || type.IsSameAs("single")) return float.Parse(value);
            else if (type.IsSameAs("int") || type.IsSameAs("int32")) return int.Parse(value);
            else if (type.IsSameAs("long") || type.IsSameAs("int64")) return long.Parse(value);
            else if (type.IsSameAs("sbyte")) return sbyte.Parse(value);
            else if (type.IsSameAs("short") || type.IsSameAs("int16")) return short.Parse(value);
            else if (type.IsSameAs("timespan")) return TimeSpan.Parse(value);
            else if (type.IsSameAs("uint") || type.IsSameAs("uint32")) return uint.Parse(value);
            else if (type.IsSameAs("ulong") || type.IsSameAs("uint64")) return ulong.Parse(value);
            else if (type.IsSameAs("ushort") || type.IsSameAs("uint16")) return ushort.Parse(value);

            return value;
        }

        private static string Replace(Match match)
        {
            GroupCollection groups = match.Groups;
            Group name = groups["name"];
            string rawValue = match.Value;

            if (!name.Success) return rawValue;

            if (!fields.TryGetValue(name.Value, out string valueText)) return rawValue;

            Group options = groups["options"];

            if (!options.Success) return valueText;

            CaptureCollection captures = options.Captures;
            string typeText = captures[0].Value;
            object value;

            try { value = Parse(valueText, typeText); }
            catch { return valueText; }

            if (captures.Count < 2) return value.ToString();

            try { return string.Format($"{{0:{captures[1].Value}}}", value); }
            catch { return value.ToString(); }
        }

        #endregion
    }
}
