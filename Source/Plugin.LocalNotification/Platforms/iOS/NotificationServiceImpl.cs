using CoreLocation;
using Foundation;
using Plugin.LocalNotification.EventArgs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UIKit;
using UserNotifications;

namespace Plugin.LocalNotification.Platforms
{
    /// <inheritdoc />
    public class NotificationServiceImpl : INotificationService
    {
        /// <inheritdoc />
        public Func<NotificationRequest, Task<NotificationEventReceivingArgs>> NotificationReceiving { get; set; }

        /// <inheritdoc />
        public event NotificationReceivedEventHandler NotificationReceived;

        /// <inheritdoc />
        public event NotificationActionTappedEventHandler NotificationActionTapped;

        /// <inheritdoc />
        public event NotificationDisabledEventHandler NotificationsDisabled;

        /// <inheritdoc />
        public void OnNotificationReceived(NotificationEventArgs e)
        {
            NotificationReceived?.Invoke(e);
        }

        /// <inheritdoc />
        public void OnNotificationActionTapped(NotificationActionEventArgs e)
        {
            NotificationActionTapped?.Invoke(e);
        }

        /// <summary>
        ///
        /// </summary>
        public void OnNotificationsDisabled()
        {
            NotificationsDisabled?.Invoke();
        }

        /// <inheritdoc />
        public bool Cancel(params int[] notificationIdList)
        {
#if XAMARINIOS
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0) == false)
            {
                return false;
            }
#elif IOS
            if (!OperatingSystem.IsAndroidVersionAtLeast(10))
            {
                return false;
            }
#endif

            var itemList = notificationIdList.Select((item) => item.ToString()).ToArray();

            UNUserNotificationCenter.Current.RemovePendingNotificationRequests(itemList);
            UNUserNotificationCenter.Current.RemoveDeliveredNotifications(itemList);

            return true;
        }

        /// <inheritdoc />
        public bool CancelAll()
        {
#if XAMARINIOS
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0) == false)
            {
                return false;
            }
#elif IOS
            if (!OperatingSystem.IsIOSVersionAtLeast(10))
            {
                return false;
            }
#endif

            UNUserNotificationCenter.Current.RemoveAllPendingNotificationRequests();
            UNUserNotificationCenter.Current.RemoveAllDeliveredNotifications();
            return true;
        }

        /// <inheritdoc />
        public bool Clear(params int[] notificationIdList)
        {
#if XAMARINIOS
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0) == false)
            {
                return false;
            }
#elif IOS
            if (!OperatingSystem.IsIOSVersionAtLeast(10))
            {
                return false;
            }
#endif

            var itemList = notificationIdList.Select((item) => item.ToString()).ToArray();

            UNUserNotificationCenter.Current.RemoveDeliveredNotifications(itemList);
            return true;
        }

        /// <inheritdoc />
        public bool ClearAll()
        {
#if XAMARINIOS
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0) == false)
            {
                return false;
            }
#elif IOS
            if (!OperatingSystem.IsIOSVersionAtLeast(10))
            {
                return false;
            }
#endif

            UNUserNotificationCenter.Current.RemoveAllDeliveredNotifications();
            return true;
        }

        /// <inheritdoc />
        public async Task<bool> Show(NotificationRequest request)
        {
            UNNotificationTrigger trigger = null;
            try
            {
#if XAMARINIOS
                if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0) == false)
                {
                    return false;
                }
#elif IOS
                if (!OperatingSystem.IsIOSVersionAtLeast(10))
                {
                    return false;
                }
#endif

                if (request is null)
                {
                    return false;
                }

                var allowed = await LocalNotificationCenter.RequestNotificationPermissionAsync().ConfigureAwait(false);
                if (allowed == false)
                {
                    LocalNotificationCenter.Log("User denied permission");
                    OnNotificationsDisabled();
                    return false;
                }

                using var content = await GetNotificationContent(request);

                var notificationId =
                    request.NotificationId.ToString(CultureInfo.CurrentCulture);

                if (request.Geofence.IsGeofence)
                {
                    var center = new CLLocationCoordinate2D(request.Geofence.Center.Latitude, request.Geofence.Center.Longitude);
                    var regin = new CLCircularRegion(center,
                                     request.Geofence.RadiusInMeters,
                                     notificationId)
                    {                        
                        NotifyOnEntry = request.Geofence.NotifyOn == NotificationRequestGeofence.GeofenceNotifyOn.OnEntry,
                        NotifyOnExit = request.Geofence.NotifyOn == NotificationRequestGeofence.GeofenceNotifyOn.OnExit
                    };                   
                    trigger = UNLocationNotificationTrigger.CreateTrigger(regin, false);
                }
                else
                {
                    var repeats = request.Schedule.RepeatType != NotificationRepeat.No;

                    if (repeats && request.Schedule.RepeatType == NotificationRepeat.TimeInterval &&
                        request.Schedule.NotifyRepeatInterval.HasValue)
                    {
                        var interval = request.Schedule.NotifyRepeatInterval.Value;

                        // Cannot delay and repeat in when TimeInterval
                        trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(interval.TotalSeconds, true);
                    }
                    else
                    {
                        using var notifyTime = GetNsDateComponentsFromDateTime(request);
                        trigger = UNCalendarNotificationTrigger.CreateTrigger(notifyTime, repeats);
                    }
                }

                var nativeRequest = UNNotificationRequest.FromIdentifier(notificationId, content, trigger);

                await UNUserNotificationCenter.Current.AddNotificationRequestAsync(nativeRequest)
                    .ConfigureAwait(false);

                return true;
            }
            finally
            {
                trigger?.Dispose();
            }
        }

        /// <inheritdoc />
        public Task<bool> AreNotificationsEnabled()
        {
            return LocalNotificationCenter.AreNotificationsEnabled();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<UNMutableNotificationContent> GetNotificationContent(NotificationRequest request)
        {
            var userInfoDictionary = new NSMutableDictionary();
            var serializedRequest = LocalNotificationCenter.GetRequestSerialize(request);
            userInfoDictionary.SetValueForKey(new NSString(serializedRequest), new NSString(LocalNotificationCenter.ReturnRequest));
            
            var content = new UNMutableNotificationContent
            {
                Title = request.Title,
                Subtitle = request.Subtitle,
                Body = request.Description,
                Badge = request.BadgeNumber,
                UserInfo = userInfoDictionary,
                Sound = UNNotificationSound.Default
            };

            if (
#if XAMARINIOS
                UIDevice.CurrentDevice.CheckSystemVersion(15, 0)
#elif IOS
                OperatingSystem.IsIOSVersionAtLeast(15)
#endif
                )
            {
                content.InterruptionLevel = request.iOS.Priority.ToNative();
                content.RelevanceScore = request.iOS.RelevanceScore;
            }
          
            // Image Attachment
            if (request.Image != null)
            {
                var nativeImage = await GetNativeImage(request.Image);
                if (nativeImage != null)
                {
                    content.Attachments = new[] { nativeImage };
                }
            }

            if (request.CategoryType != NotificationCategoryType.None)
            {
                content.CategoryIdentifier = request.CategoryType.ToNative();
            }

            if (string.IsNullOrWhiteSpace(request.Group) == false)
            {
                content.ThreadIdentifier = request.Group;
            }


            if (string.IsNullOrWhiteSpace(request.iOS.SummaryArgument) == false)
            {
                if (
#if XAMARINIOS
                     UIDevice.CurrentDevice.CheckSystemVersion(12, 0)
#elif IOS
                     OperatingSystem.IsIOSVersionAtLeast(12)
#endif                
                   )
                {
                    content.SummaryArgument = request.iOS.SummaryArgument;
                    content.SummaryArgumentCount = (nuint)request.iOS.SummaryArgumentCount;
                }
            }

            if (string.IsNullOrWhiteSpace(request.Sound) == false)
            {
                content.Sound = UNNotificationSound.GetSound(request.Sound);
            }

            if (request.Silent)
            {
                content.Sound = null;
            }
            return content;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="notificationImage"></param>
        /// <returns></returns>
        protected virtual async Task<UNNotificationAttachment> GetNativeImage(NotificationImage notificationImage)
        {
            if (notificationImage is null || notificationImage.HasValue == false)
            {
                return null;
            }

            NSUrl imageAttachment = null;
            if (string.IsNullOrWhiteSpace(notificationImage.ResourceName) == false)
            {
                imageAttachment = NSBundle.MainBundle.GetUrlForResource(
                    Path.GetFileNameWithoutExtension(notificationImage.ResourceName),
                    Path.GetExtension(notificationImage.ResourceName));
            }

            if (string.IsNullOrWhiteSpace(notificationImage.FilePath) == false)
            {
                if (File.Exists(notificationImage.FilePath))
                {
                    imageAttachment = NSUrl.CreateFileUrl(notificationImage.FilePath, false, null);
                }
            }

            if (notificationImage.Binary is { Length: > 0 })
            {
                var cache = NSSearchPath.GetDirectories(NSSearchPathDirectory.CachesDirectory,
                    NSSearchPathDomain.User);
                var cachesFolder = cache[0];
                var cacheFile = $"{cachesFolder}{NSProcessInfo.ProcessInfo.GloballyUniqueString}";

                if (File.Exists(cacheFile))
                {
                    File.Delete(cacheFile);
                }

                await File.WriteAllBytesAsync(cacheFile, notificationImage.Binary);

                imageAttachment = NSUrl.CreateFileUrl(cacheFile, false, null);
            }

            if (imageAttachment is null)
            {
                return null;
            }

            var options = new UNNotificationAttachmentOptions();

            return UNNotificationAttachment.FromIdentifier("image", imageAttachment, options, out _);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="notificationRequest"></param>
        /// <returns></returns>
        protected static NSDateComponents GetNsDateComponentsFromDateTime(NotificationRequest notificationRequest)
        {
            var dateTime = notificationRequest.Schedule.NotifyTime ?? DateTime.Now.AddSeconds(1);

            return notificationRequest.Schedule.RepeatType switch
            {
                NotificationRepeat.Daily => new NSDateComponents
                {
                    Hour = dateTime.Hour,
                    Minute = dateTime.Minute,
                    Second = dateTime.Second
                },
                NotificationRepeat.Weekly => new NSDateComponents
                {
                    // iOS: Weekday units are the numbers 1 through n, where n is the number of days in the week.
                    // For example, in the Gregorian calendar, n is 7 and Sunday is represented by 1.
                    // .Net: The returned value is an integer between 0 and 6,
                    // where 0 indicates Sunday, 1 indicates Monday, 2 indicates Tuesday, 3 indicates Wednesday, 4 indicates Thursday, 5 indicates Friday, and 6 indicates Saturday.
                    Weekday = (int)dateTime.DayOfWeek + 1,
                    Hour = dateTime.Hour,
                    Minute = dateTime.Minute,
                    Second = dateTime.Second
                },
                NotificationRepeat.No => new NSDateComponents
                {
                    Day = dateTime.Day,
                    Month = dateTime.Month,
                    Year = dateTime.Year,
                    Hour = dateTime.Hour,
                    Minute = dateTime.Minute,
                    Second = dateTime.Second
                },
                _ => new NSDateComponents
                {
                    Day = dateTime.Day,
                    Hour = dateTime.Hour,
                    Minute = dateTime.Minute,
                    Second = dateTime.Second
                }
            };
        }

        /// <inheritdoc />
        public void RegisterCategoryList(HashSet<NotificationCategory> categoryList)
        {
            if (categoryList is null || categoryList.Any() == false)
            {
                return;
            }

            var nativeCategoryList = new List<UNNotificationCategory>();
            foreach (var category in categoryList)
            {
                if (category.CategoryType == NotificationCategoryType.None)
                {
                    continue;
                }

                var nativeCategory = RegisterActionList(category);
                if (nativeCategory != null)
                {
                    nativeCategoryList.Add(nativeCategory);
                }
            }

            if (nativeCategoryList.Any() == false)
            {
                return;
            }

            UNUserNotificationCenter.Current.SetNotificationCategories(
                new NSSet<UNNotificationCategory>(nativeCategoryList.ToArray()));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        protected static UNNotificationCategory RegisterActionList(NotificationCategory category)
        {
            if (category is null || category.CategoryType == NotificationCategoryType.None)
            {
                return null;
            }

            var nativeActionList = new List<UNNotificationAction>();
            foreach (var notificationAction in category.ActionList)
            {
                if (notificationAction.ActionId == -1000)
                {
                    continue;
                }

                var nativeAction = UNNotificationAction.FromIdentifier(
                    notificationAction.ActionId.ToString(CultureInfo.InvariantCulture), notificationAction.Title,
                    notificationAction.iOS.Action.ToNative());
                nativeActionList.Add(nativeAction);
            }

            if (nativeActionList.Any() == false)
            {
                return null;
            }

            var notificationCategory = UNNotificationCategory
                .FromIdentifier(category.CategoryType.ToNative(), nativeActionList.ToArray(),
                    Array.Empty<string>(), UNNotificationCategoryOptions.CustomDismissAction);

            return notificationCategory;
        }
                        
        /// <inheritdoc />
        public async Task<IList<NotificationRequest>> GetPendingNotificationList()
        {
            var pending = await UNUserNotificationCenter.Current.GetPendingNotificationRequestsAsync();

            return pending.Select(r => GetRequest(r.Content)).ToList();
        }

        /// <inheritdoc />
        public async Task<IList<NotificationRequest>> GetDeliveredNotificationList()
        {
            var delivered = await UNUserNotificationCenter.Current.GetDeliveredNotificationsAsync();

            return delivered.Select(r => GetRequest(r.Request.Content)).ToList();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="notificationContent"></param>
        /// <returns></returns>
        public NotificationRequest GetRequest(UNNotificationContent notificationContent)
        {
            if (notificationContent is null)
            {
                return null;
            }

            var dictionary = notificationContent.UserInfo;

            if (!dictionary.ContainsKey(new NSString(LocalNotificationCenter.ReturnRequest)))
            {
                return null;
            }

            var requestSerialize = dictionary[LocalNotificationCenter.ReturnRequest].ToString();

            var request = LocalNotificationCenter.GetRequest(requestSerialize);

            return request;
        }
    }
}