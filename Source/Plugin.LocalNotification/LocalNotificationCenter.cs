using Microsoft.Extensions.Logging;
using Plugin.LocalNotification.Json;

#if ANDROID || IOS || WINDOWS
using Plugin.LocalNotification.Platforms;
#endif

namespace Plugin.LocalNotification;

/// <summary>
/// Provides a cross-platform resolver for <see cref="INotificationService"/> and manages notification serialization and logging.
/// </summary>
public partial class LocalNotificationCenter
{
    private static readonly Lazy<INotificationService?> implementation = new(CreateNotificationService, LazyThreadSafetyMode.PublicationOnly);
    private static INotificationSerializer? _serializer;

    private static INotificationService? CreateNotificationService() =>
#if ANDROID || IOS || WINDOWS
        new NotificationServiceImpl();
#else
        null;
#endif

    /// <summary>
    /// Gets or sets the internal logger for notification events.
    /// </summary>
    internal static ILogger? Logger { get; set; }

    /// <summary>
    /// Gets or sets the log level for internal logging.
    /// </summary>
    public static LogLevel LogLevel { get; set; } = LogLevel.Trace;

    /// <summary>
    /// Gets the platform-specific <see cref="INotificationService"/> implementation.
    /// </summary>
    public static INotificationService Current => implementation.Value;

    /// <summary>
    /// The key used to return a notification request.
    /// </summary>
    public const string ReturnRequest = "Plugin.LocalNotification.RETURN_REQUEST";

    /// <summary>
    /// The key used to return a notification action id.
    /// </summary>
    public const string ReturnRequestActionId = "Plugin.LocalNotification.RETURN_ActionId";

    /// <summary>
    /// The key used to indicate a notification was handled.
    /// </summary>
    public const string ReturnRequestHandled = "Plugin.LocalNotification.RETURN_Handled";

    /// <summary>
    /// Gets or sets the serializer used for notification requests.
    /// </summary>
    internal static INotificationSerializer Serializer
    {
        get
        {
            _serializer ??= new NotificationSerializer();
            return _serializer;
        }
        set => _serializer = value;
    }

    /// <summary>
    /// Deserializes a notification request from its serialized string representation.
    /// </summary>
    /// <param name="serializedRequest">The serialized notification request string.</param>
    /// <returns>The deserialized <see cref="NotificationRequest"/>.</returns>
    internal static NotificationRequest GetRequest(string? serializedRequest)
    {
        Logger?.LogTrace("Serialized Request [{serializedRequest}]", serializedRequest);
        if (string.IsNullOrWhiteSpace(serializedRequest))
        {
            return new NotificationRequest();
        }

        var request = Serializer.Deserialize<NotificationRequest>(serializedRequest);
        return request ?? new NotificationRequest();
    }

    /// <summary>
    /// Deserializes a list of notification requests from its serialized string representation.
    /// </summary>
    /// <param name="serializedRequestList">The serialized list of notification requests.</param>
    /// <returns>The deserialized list of <see cref="NotificationRequest"/> objects.</returns>
    internal static List<NotificationRequest> GetRequestList(string? serializedRequestList)
    {
        if (string.IsNullOrWhiteSpace(serializedRequestList))
        {
            return [];
        }

        var requestList = Serializer.Deserialize<List<NotificationRequest>>(serializedRequestList);
        return requestList ?? [];
    }

    /// <summary>
    /// Serializes a list of notification requests to a string.
    /// </summary>
    /// <param name="requestList">The list of notification requests to serialize.</param>
    /// <returns>The serialized string representation of the list.</returns>
    internal static string GetRequestListSerialize(List<NotificationRequest> requestList)
    {
        if (requestList is null || requestList.Count <= 0)
        {
            // Return an empty JSON array if the list is null or empty
            return "[]";
        }

        foreach (var request in requestList)
        {
            if (request.Image is not null &&
                request.Image.Binary is not null &&
                request.Image.Binary?.Length > 90000)
            {
                request.Image.Binary = [];
            }
        }
        var serializedRequestList = Serializer.Serialize(requestList);
        return serializedRequestList;
    }

    /// <summary>
    /// Serializes a notification request to a string.
    /// </summary>
    /// <param name="request">The notification request to serialize.</param>
    /// <returns>The serialized string representation of the request.</returns>
    internal static string GetRequestSerialize(NotificationRequest request)
    {
        if (request is null)
        {
            // Return an empty JSON array if the list is null or empty
            return "[]";
        }

        if (request.Image is not null &&
            request.Image.Binary is not null &&
            request.Image.Binary?.Length > 90000)
        {
            request.Image.Binary = [];
        }
        var serializedRequest = Serializer.Serialize(request);

        Logger?.LogTrace("Serialized Request [{serializedRequest}]", serializedRequest);

        return serializedRequest;
    }
}