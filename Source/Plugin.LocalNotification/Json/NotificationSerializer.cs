using System.Text.Json;

namespace Plugin.LocalNotification.Json
{
    /// <inheritdoc />
    public class NotificationSerializer : INotificationSerializer
    {
        /// <summary>
        ///
        /// </summary>
        public virtual JsonSerializerOptions MyJsonSerializerOptions { get; } = new JsonSerializerOptions
        {
            Converters =
            {
                new JsonValueConverterTimeSpan()
            }
        };

        /// <inheritdoc />
        public virtual TValue Deserialize<TValue>(string json)
        {
            return JsonSerializer.Deserialize<TValue>(json, MyJsonSerializerOptions);
        }

        /// <inheritdoc />
        public virtual string Serialize<TValue>(TValue value)
        {
            return JsonSerializer.Serialize(value, MyJsonSerializerOptions);
        }
    }
}