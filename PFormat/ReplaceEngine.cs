using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PFormat
{
    internal class ReplaceEngine
    {
        #region Private Fields

        private Dictionary<string, string> fields;

        #endregion

        #region Internal Methods

        internal static string FixLinefeedCode(string value)
        {
            return Regex.Replace(value, "\\n(?<=[^\\r])", Environment.NewLine);
        }

        internal static string Replace(string format, Dictionary<string, string> fields)
        {
            var engine = new ReplaceEngine(fields);
            engine.ExpandFields();
            return engine.Replace(format);
        }

        #endregion

        #region Private Methods

        private ReplaceEngine(Dictionary<string, string> fields)
        {
            this.fields = fields;
        }

        private bool AreSame(string text1, string text2)
        {
            return (string.Compare(text1, text2, true) == 0);
        }

        private void ExpandFields()
        {
            var result = new Dictionary<string, string>(fields.Count);

            foreach (string key in fields.Keys)
            {
                result[key] = Replace(fields[key]);
            }

            fields = result;
        }

        private object Parse(string value, string type)
        {
            if (AreSame(type, "bool") || AreSame(type, "boolean")) return bool.Parse(value);
            else if (AreSame(type, "byte")) return byte.Parse(value);
            else if (AreSame(type, "char")) return char.Parse(value);
            else if (AreSame(type, "datetime")) return DateTime.Parse(value);
            else if (AreSame(type, "decimal")) return decimal.Parse(value);
            else if (AreSame(type, "double")) return double.Parse(value);
            else if (AreSame(type, "float") || AreSame(type, "single")) return float.Parse(value);
            else if (AreSame(type, "int") || AreSame(type, "int32")) return int.Parse(value);
            else if (AreSame(type, "long") || AreSame(type, "int64")) return long.Parse(value);
            else if (AreSame(type, "sbyte")) return sbyte.Parse(value);
            else if (AreSame(type, "short") || AreSame(type, "int16")) return short.Parse(value);
            else if (AreSame(type, "timespan")) return TimeSpan.Parse(value);
            else if (AreSame(type, "uint") || AreSame(type, "uint32")) return uint.Parse(value);
            else if (AreSame(type, "ulong") || AreSame(type, "uint64")) return ulong.Parse(value);
            else if (AreSame(type, "ushort") || AreSame(type, "uint16")) return ushort.Parse(value);

            return value;
        }

        private string Replace(Match match)
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

        private string Replace(string format)
        {
            return Regex.Replace(format, "\\$[{]([{](?<options>[^}]+)[}])*(?<name>[^}]+)[}]", Replace);
        }

        #endregion
    }
}
