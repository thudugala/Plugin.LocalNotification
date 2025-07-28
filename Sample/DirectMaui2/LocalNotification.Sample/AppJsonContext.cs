using Plugin.LocalNotification;
using System.Text.Json.Serialization;

namespace LocalNotification.Sample;

[JsonSourceGenerationOptions(
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals)]
[JsonSerializable(typeof(List<string>))]
[JsonSerializable(typeof(NotificationRequest))]
internal partial class AppJsonContext : JsonSerializerContext
{
}
