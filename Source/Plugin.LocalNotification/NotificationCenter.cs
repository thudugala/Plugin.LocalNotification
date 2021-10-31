using Plugin.LocalNotification.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Plugin.LocalNotification
{
    /// <summary>
    /// Cross platform INotificationService Resolver.
    /// </summary>

    public static partial class NotificationCenter
    {
        private static INotificationService _current;
        private static INotificationSerializer _serializer;

        /// <summary>
        /// Internal Error happened
        /// </summary>
        public static event NotificationLogHandler NotificationLog;

        /// <summary>
        /// Platform specific INotificationService.
        /// </summary>
        public static INotificationService Current
        {
            get =>
                _current ?? throw new InvalidOperationException(Properties.Resources.PluginNotFound);
            set => _current = value;
        }

        /// <summary>
        /// Return Notification Key.
        /// </summary>
        public static string ReturnRequest => "Plugin.LocalNotification.RETURN_REQUEST";

        /// <summary>
        ///
        /// </summary>
        public static INotificationSerializer Serializer
        {
            get => _serializer ?? throw new InvalidOperationException(Properties.Resources.PluginSerializerNotFound);
            set => _serializer = value;
        }

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

        internal static Dictionary<string, string> GetRequestSerializeDictionary(NotificationRequest request)
        {
            var dictionary = new Dictionary<string, string>();

            if (request.Image.Binary != null && request.Image.Binary.Length > 90000)
            {
                request.Image.Binary = null;
            }
            var serializedRequest = GetRequestSerialize(request);
            dictionary.Add(ReturnRequest, serializedRequest);
            return dictionary;
        }
    }
}