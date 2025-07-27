using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Plugin.LocalNotification.Json;

/// <inheritdoc />
internal class NotificationSerializer : INotificationSerializer
{
    private readonly NotificationJsonContext _context = new ();

    /// <inheritdoc />
    public virtual TValue? Deserialize<TValue>(string json)
    {
        return _context.GetTypeInfo(typeof(TValue)) is not JsonTypeInfo<TValue> jsonTypeInfo
            ? throw new InvalidOperationException($"Type information for {typeof(TValue).FullName} is not available.")
            : JsonSerializer.Deserialize<TValue>(json, jsonTypeInfo);
    }

    /// <inheritdoc />
    public virtual string Serialize<TValue>(TValue value)
    {
        var jsonTypeInfo = _context.GetTypeInfo(typeof(TValue));
        return jsonTypeInfo is null
            ? throw new InvalidOperationException($"Type information for {typeof(TValue).FullName} is not available.")
            : JsonSerializer.Serialize(value, jsonTypeInfo);
    }
}