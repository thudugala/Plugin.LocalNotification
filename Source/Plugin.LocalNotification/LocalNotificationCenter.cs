using Plugin.LocalNotification.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Logging;
#if ANDROID || IOS || MONOANDROID || XAMARINIOS
using Plugin.LocalNotification.Platforms;
#endif

namespace Plugin.LocalNotification
{
    /// <summary>
    /// Cross platform INotificationService Resolver.
    /// </summary>

    public partial class LocalNotificationCenter
    {
        private static readonly Lazy<INotificationService> implementation = new(() => CreateNotificationService(), LazyThreadSafetyMode.PublicationOnly);
        private static INotificationSerializer _serializer;

        private static INotificationService CreateNotificationService()
        {
#if ANDROID || IOS || MONOANDROID || XAMARINIOS
            return new NotificationServiceImpl();
#else
            return null;
#endif
        }

        /// <summary>
        /// Internal  Logger
        /// </summary>
        public static ILogger Logger { get; set; }

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
        public static INotificationSerializer Serializer
        {
            get
            {
                _serializer ??= new NotificationSerializer();
                return _serializer;
            }
            set => _serializer = value;
        }

        internal static NotificationRequest GetRequest(string serializedRequest)
        {
            Logger?.LogTrace($"Serialized Request [{serializedRequest}]");
            if (string.IsNullOrWhiteSpace(serializedRequest))
            {
                return null;
            }

            var request = Serializer.Deserialize<NotificationRequest>(serializedRequest);
            return request;
        }

        internal static List<NotificationRequest> GetRequestList(string serializedRequestList)
        {
            if (string.IsNullOrWhiteSpace(serializedRequestList))
            {
                return new List<NotificationRequest>();
            }

            var requestList = Serializer.Deserialize<List<NotificationRequest>>(serializedRequestList);
            return requestList;
        }

        internal static string GetRequestListSerialize(List<NotificationRequest> requestList)
        {
            foreach (var request in requestList)
            {
                if (request.Image.Binary != null && request.Image.Binary.Length > 90000)
                {
                    request.Image.Binary = null;
                }
            }
            var serializedRequestList = Serializer.Serialize(requestList);
            return serializedRequestList;
        }

        internal static string GetRequestSerialize(NotificationRequest request)
        {
            if (request.Image.Binary != null && request.Image.Binary.Length > 90000)
            {
                request.Image.Binary = null;
            }
            var serializedRequest = Serializer.Serialize(request);

            Logger?.LogTrace($"Serialized Request [{serializedRequest}]");

            return serializedRequest;
        }
    }
}