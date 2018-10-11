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
        public static string SerializeObject(T returningData)
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
        public static T DeserializeObject(string returningData)
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