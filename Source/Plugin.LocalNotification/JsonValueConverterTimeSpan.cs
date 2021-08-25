using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Plugin.LocalNotification
{
    /// <summary>
    ///
    /// </summary>
    public class JsonValueConverterTimeSpan : JsonConverter<TimeSpan>
    {
        /// <inheritdoc />
        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                var value = reader.GetString();
                return string.IsNullOrWhiteSpace(value) ? TimeSpan.MinValue : TimeSpan.Parse(value, CultureInfo.InvariantCulture);
            }
            catch
            {
                return TimeSpan.MinValue;
            }
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            try
            {
                var valueStr = value.ToString();
                writer?.WriteStringValue(valueStr);
            }
            catch
            {
                //Ignore
            }
        }
    }
}