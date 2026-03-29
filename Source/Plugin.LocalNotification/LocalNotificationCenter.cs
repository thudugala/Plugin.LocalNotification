using Plugin.LocalNotification.Core;
using Plugin.LocalNotification.Core.Models;
using Plugin.LocalNotification.Json;
using Plugin.LocalNotification.Platforms;

namespace Plugin.LocalNotification;

/// <summary>
/// Provides a cross-platform resolver for <see cref="INotificationService"/> and manages notification serialization and logging.
/// </summary>
public partial class LocalNotificationCenter
{
    private static Lazy<INotificationService> implementation = new(CreateNotificationService, LazyThreadSafetyMode.PublicationOnly);

    private static INotificationService CreateNotificationService()
    {
        return new NotificationServiceImpl();
    }

    /// <summary>
    /// Allows setting a custom notification service implementation. Use this to enable optional features like geofence.
    /// </summary>
    /// <param name="service">The service implementation to use.</param>
    public static void SetNotificationService(INotificationService service)
    {
        implementation = new Lazy<INotificationService>(() => service, LazyThreadSafetyMode.PublicationOnly);
    }
        
    /// <summary>
    /// Gets the platform-specific <see cref="INotificationService"/> implementation.
    /// </summary>
    public static INotificationService Current => implementation.Value!;

    /// <summary>
    /// Gets the Android-specific notification service, or <c>null</c> on non-Android platforms.
    /// Use this to access <see cref="IAndroidNotificationService"/> APIs such as channel management
    /// and exact-alarm permission requests.
    /// </summary>
    public static IAndroidNotificationService? AndroidService => Current as IAndroidNotificationService;
        
    /// <summary>
    /// Gets or sets the serializer used for notification requests.
    /// </summary>
    internal static INotificationSerializer Serializer
    {
        get
        {
            field ??= new NotificationSerializer();
            return field;
        }
        set;
    }

    /// <summary>
    /// Deserializes a notification request from its serialized string representation.
    /// </summary>
    /// <param name="serializedRequest">The serialized notification request string.</param>
    /// <returns>The deserialized <see cref="NotificationRequest"/>.</returns>
    internal static NotificationRequest GetRequest(string? serializedRequest)
    {
        LocalNotificationLogger.Log($"Serialized Request [{serializedRequest}]");
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

        LocalNotificationLogger.Log($"Serialized Request [{serializedRequest}]");

        return serializedRequest;
    }
}