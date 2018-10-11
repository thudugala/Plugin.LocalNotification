using System.IO;
using System.Xml.Serialization;

namespace Plugin.LocalNotification
{
    /// <summary>
    ///
    /// </summary>
    public static class DataSerializer<T>
    {
        /// <summary>
        /// Serialize Returning Data
        /// </summary>
        /// <param name="returningData"></param>
        /// <returns></returns>
        public static string SerializeReturningData(T returningData)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            using (var stringWriter = new StringWriter())
            {
                xmlSerializer.Serialize(stringWriter, returningData);
                return stringWriter.ToString();
            }
        }

        /// <summary>
        /// Deserialize Returning Data.
        /// </summary>
        /// <param name="returningData"></param>
        /// <returns></returns>
        public static T DeserializeReturningData(string returningData)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            using (var stringReader = new StringReader(returningData))
            {
                var notification = (T)xmlSerializer.Deserialize(stringReader);
                return notification;
            }
        }
    }
}