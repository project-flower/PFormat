using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace PFormat
{
    internal static class SaveFileNamager
    {
        #region Private Fields

        private static readonly XmlSerializer serializer = new XmlSerializer(typeof(ApplicationData));

        #endregion

        #region Internal Methods

        internal static ApplicationData LoadFromFile(string fileName)
        {
            ApplicationData result;

            using (var reader = new StreamReader(fileName, Encoding.UTF8))
            {
                result = (serializer.Deserialize(reader) as ApplicationData);
            }

            // XML 要素は CRLF をサポートしないため、変換する。
            foreach (ApplicationData.Group group in result.Groups)
            {
                (string, string)[] formats = group.Formats;

                for (int i = 0; i < formats.Length; ++i)
                {
                    // (string, string) format = formats[i]; タプルは値型のためこれはダメ
                    formats[i].Item2 = ReplaceEngine.FixLinefeedCode(formats[i].Item2);
                }
            }

            return result;
        }

        internal static void SaveToFile(string fileName, ApplicationData data)
        {
            using (var stream = new FileStream(fileName, FileMode.Create))
            using (var writer = new StreamWriter(stream, Encoding.UTF8))
            {
                serializer.Serialize(writer, data);
            }
        }

        #endregion
    }
}
