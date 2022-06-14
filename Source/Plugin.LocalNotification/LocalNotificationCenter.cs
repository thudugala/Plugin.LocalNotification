using Plugin.LocalNotification.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Plugin.LocalNotification.EventArgs;
using System.Threading;
#if !NETSTANDARD2_0
using Plugin.LocalNotification.Platforms;
#endif

namespace Plugin.LocalNotification
{
    /// <summary>
    /// Cross platform INotificationService Resolver.
    /// </summary>

    public partial class LocalNotificationCenter
    {
        private static readonly Lazy<INotificationService> implementation = new (() => CreateNotificationService(), LazyThreadSafetyMode.PublicationOnly);

        private static INotificationService CreateNotificationService()
        {
            Serializer = new NotificationSerializer();
#if NETSTANDARD2_0
            return null;
#else
			return new NotificationServiceImpl();
#endif
            
        }

        /// <summary>
        /// Internal Error happened
        /// </summary>
        public static event NotificationLogHandler NotificationLog;

        /// <summary>
        /// Platform specific INotificationService.
        /// </summary>
        public static INotificationService Current
        {
            get
            {
                var ret = implementation.Value;
                if (ret == null)
                {
                    throw new NotImplementedException(Properties.Resources.PluginNotFound);
                }
                return ret;
            }
        }

        /// <summary>
        /// Return Notification Key.
        /// </summary>
        public static string ReturnRequest => "Plugin.LocalNotification.RETURN_REQUEST";

        /// <summary>
        /// Return Notification Handled Key
        /// </summary>
        public static string ReturnRequestHandled => "Plugin.LocalNotification.RETURN_Handled";

        /// <summary>
        ///
        /// </summary>
        public static INotificationSerializer Serializer { get; set; }

        internal static NotificationRequest GetRequest(string serializedRequest)
        {
            Debug.WriteLine($"Serialized Request [{serializedRequest}]");
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

            Debug.WriteLine($"Serialized Request [{serializedRequest}]");

            return serializedRequest;
        }
    }
}
