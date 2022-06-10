#if NETSTANDARD || MONOANDROID || XAMARINIOS
using Newtonsoft.Json;
#else
using System.Text.Json;
#endif

namespace Plugin.LocalNotification.Json
{
    /// <inheritdoc />
    public class NotificationSerializer : INotificationSerializer
    {
        /// <inheritdoc />
        public virtual TValue Deserialize<TValue>(string json)
        {
#if NETSTANDARD || MONOANDROID || XAMARINIOS
            return JsonConvert.DeserializeObject<TValue>(json);
#else
            return JsonSerializer.Deserialize<TValue>(json);
#endif
        }

        /// <inheritdoc />
        public virtual string Serialize<TValue>(TValue value)
        {
#if NETSTANDARD || MONOANDROID || XAMARINIOS
            return JsonConvert.SerializeObject(value);
#else
            return JsonSerializer.Serialize(value);
#endif
        }
    }
}