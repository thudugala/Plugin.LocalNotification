using System.Text.Json;

namespace Plugin.LocalNotification.Json
{
    /// <inheritdoc />
    public class NotificationSerializer : INotificationSerializer
    {
        /// <inheritdoc />
        public virtual TValue? Deserialize<TValue>(string json)
        {
            return JsonSerializer.Deserialize<TValue>(json);
        }

        /// <inheritdoc />
        public virtual string Serialize<TValue>(TValue value)
        {
            return JsonSerializer.Serialize(value);
        }
    }
}