using System.Text.Json;
using System.Text.Json.Serialization;

namespace Plugin.LocalNotification.Json;

/// <inheritdoc />
internal class NotificationSerializer : INotificationSerializer
{
    private readonly JsonSerializerOptions _options = new()
    {
        NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
    };

    /// <inheritdoc />
    public virtual TValue? Deserialize<TValue>(string json)
    {
        return JsonSerializer.Deserialize<TValue>(json, _options);
    }

    /// <inheritdoc />
    public virtual string Serialize<TValue>(TValue value)
    {
        return JsonSerializer.Serialize(value, _options);
    }
}