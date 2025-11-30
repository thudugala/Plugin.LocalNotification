using System.Text.Json.Serialization;

namespace Plugin.LocalNotification.Json;

/// <summary>
/// Source generation context for System.Text.Json serialization of notification types.
/// </summary>
[JsonSourceGenerationOptions(
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals)]
[JsonSerializable(typeof(NotificationRequest))]
[JsonSerializable(typeof(List<NotificationRequest>))]
[JsonSerializable(typeof(NotificationImage))]
[JsonSerializable(typeof(NotificationRequestGeofence))]
[JsonSerializable(typeof(NotificationRequestGeofence.Position))]
internal partial class NotificationJsonContext : JsonSerializerContext
{
}
