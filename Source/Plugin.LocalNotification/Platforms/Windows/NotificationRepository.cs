using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;

namespace Plugin.LocalNotification.Platforms
{
    /// <summary>
    ///
    /// </summary>
    internal class NotificationRepository
    {
        private static readonly Lazy<NotificationRepository> MySingleton =
            new(() => new NotificationRepository(),
                System.Threading.LazyThreadSafetyMode.PublicationOnly);

        private static readonly object Locker = new object();

        /// <summary>
        ///
        /// </summary>
        internal static NotificationRepository Current => MySingleton.Value;

        /// <summary>
        ///
        /// </summary>
        private const string PendingListKey = $"{nameof(NotificationRepository)}PendingList";

        /// <summary>
        ///
        /// </summary>
        private const string DeliveredListKey = $"{nameof(NotificationRepository)}DeliveredList";

        static ApplicationDataContainer GetApplicationDataContainer()
        {
            var localSettings = ApplicationData.Current.LocalSettings; 
            return localSettings;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="notificationIdList"></param>
        internal void RemoveByPendingIdList(params int[] notificationIdList)
        {
            var itemList = GetPendingList();
            itemList.RemoveAll(r => notificationIdList.Contains(r.NotificationId));
            SetPendingList(itemList);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        internal void AddPendingRequest(NotificationRequest request)
        {
            var itemList = GetPendingList();
            itemList.RemoveAll(r => request.NotificationId == r.NotificationId);
            itemList.RemoveAll(r =>
                r.Schedule.NotifyTime.HasValue &&
                r.Schedule.Android.IsValidNotifyTime(DateTime.Now, r.Schedule.NotifyTime) == false);
            itemList.Add(request);
            SetPendingList(itemList);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        internal void AddDeliveredRequest(NotificationRequest request)
        {
            var itemList = GetDeliveredList();
            itemList.RemoveAll(r => request.NotificationId == r.NotificationId);
            itemList.Add(request);
            SetDeliveredList(itemList);
        }

        /// <summary>
        ///
        /// </summary>
        internal void RemoveDeliveredList()
        {
            SetDeliveredList(null);
        }

        /// <summary>
        ///
        /// </summary>
        internal void RemovePendingList()
        {
            SetPendingList(null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="notificationIdList"></param>
        internal void RemoveByDeliveredIdList(params int[] notificationIdList)
        {
            var itemList = GetDeliveredList();
            itemList.RemoveAll(r => notificationIdList.Contains(r.NotificationId));
            SetDeliveredList(itemList);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        internal List<NotificationRequest> GetPendingList()
        {
            var itemList = GetList(PendingListKey);
            return itemList;
        }

        private static void SetPendingList(List<NotificationRequest> list)
        {
            SetList(PendingListKey, list);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        internal List<NotificationRequest> GetDeliveredList()
        {
            var itemList = GetList(DeliveredListKey);
            return itemList;
        }

        private static void SetDeliveredList(List<NotificationRequest> list)
        {
            SetList(DeliveredListKey, list);
        }

        private static List<NotificationRequest> GetList(string key)
        {
            lock (Locker)
            {
                var appDataContainer = GetApplicationDataContainer();
                var jsonText = string.Empty;
                if (appDataContainer.Values.TryGetValue(key, out var data))
                {
                   jsonText = data.ToString();
                }
                return string.IsNullOrWhiteSpace(jsonText)
                    ? new List<NotificationRequest>()
                    : LocalNotificationCenter.GetRequestList(jsonText);
            }
        }

        private static void SetList(string key, List<NotificationRequest> list)
        {
            lock (Locker)
            {
                var appDataContainer = GetApplicationDataContainer();
                string jsonText = null;
                if (list != null && list.Any())
                {
                    jsonText = LocalNotificationCenter.GetRequestListSerialize(list);
                }

                if (appDataContainer.Values.ContainsKey(key))
                {
                    appDataContainer.Values[key] = jsonText;
                }
                else
                {
                    appDataContainer.Values.Add(key, jsonText);
                }
            }
        }
    }
}