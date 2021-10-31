using Newtonsoft.Json;

namespace Plugin.LocalNotification.Json
{
    /// <inheritdoc />
    public class NotificationSerializer : INotificationSerializer
    {
        /// <inheritdoc />
        public virtual TValue Deserialize<TValue>(string json)
        {
            return JsonConvert.DeserializeObject<TValue>(json);
        }

        /// <inheritdoc />
        public virtual string Serialize<TValue>(TValue value)
        {
            return JsonConvert.SerializeObject(value);
        }
    }
}