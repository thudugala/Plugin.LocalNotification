using Plugin.LocalNotification.Json;
using Microsoft.Extensions.Logging;

#if ANDROID || IOS || WINDOWS
using Plugin.LocalNotification.Platforms;
#endif

namespace Plugin.LocalNotification
{
    /// <summary>
    /// Cross platform INotificationService Resolver.
    /// </summary>

    public partial class LocalNotificationCenter
    {
        private static readonly Lazy<INotificationService> implementation = new(CreateNotificationService, LazyThreadSafetyMode.PublicationOnly);
        private static INotificationSerializer? _serializer;

        private static INotificationService CreateNotificationService()
        {
#if ANDROID || IOS || WINDOWS
            return new NotificationServiceImpl();
#else
            return null;
#endif
        }

        /// <summary>
        /// Internal  Logger
        /// </summary>
        internal static ILogger? Logger { get; set; }

        /// <summary>
        /// Internal  Logger LogLevel
        /// </summary>
        public static LogLevel LogLevel { get; set; } = LogLevel.Trace;

        /// <summary>
        /// Platform specific INotificationService.
        /// </summary>
        public static INotificationService Current
        {
            get
            {
                var ret = implementation.Value;
                return ret ?? throw new NotImplementedException(Properties.Resources.PluginNotFound);
            }
        }

        /// <summary>
        /// Return Notification Key.
        /// </summary>
        public const string ReturnRequest = "Plugin.LocalNotification.RETURN_REQUEST";

        /// <summary>
        /// Return Notification Action Id.
        /// </summary>
        public const string ReturnRequestActionId = "Plugin.LocalNotification.RETURN_ActionId";

        /// <summary>
        /// Return Notification Handled Key
        /// </summary>
        public const string ReturnRequestHandled = "Plugin.LocalNotification.RETURN_Handled";

        /// <summary>
        ///
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

        internal static NotificationRequest GetRequest(string? serializedRequest)
        {
            Logger?.LogTrace($"Serialized Request [{serializedRequest}]");
            if (string.IsNullOrWhiteSpace(serializedRequest))
            {
                return new NotificationRequest();
            }

            var request = Serializer.Deserialize<NotificationRequest>(serializedRequest);
            return request ?? new NotificationRequest();
        }

        internal static List<NotificationRequest> GetRequestList(string? serializedRequestList)
        {
            if (string.IsNullOrWhiteSpace(serializedRequestList))
            {
                return [];
            }

            var requestList = Serializer.Deserialize<List<NotificationRequest>>(serializedRequestList);
            return requestList ?? [];
        }

        internal static string GetRequestListSerialize(List<NotificationRequest> requestList)
        {
            foreach (var request in requestList)
            {
                if (request.Image.Binary != null && request.Image.Binary.Length > 90000)
                {
                    request.Image.Binary = [];
                }
            }
            var serializedRequestList = Serializer.Serialize(requestList);
            return serializedRequestList;
        }

        internal static string GetRequestSerialize(NotificationRequest request)
        {
            if (request.Image.Binary != null && request.Image.Binary.Length > 90000)
            {
                request.Image.Binary = [];
            }
            var serializedRequest = Serializer.Serialize(request);

            Logger?.LogTrace($"Serialized Request [{serializedRequest}]");

            return serializedRequest;
        }
    }
}