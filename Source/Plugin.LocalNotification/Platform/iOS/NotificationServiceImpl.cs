using Foundation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UIKit;
using UserNotifications;

namespace Plugin.LocalNotification.Platform.iOS
{
    /// <inheritdoc />
    public class NotificationServiceImpl : INotificationService
    {
        /// <inheritdoc />
        public event NotificationTappedEventHandler NotificationTapped;

        /// <inheritdoc />
        public event NotificationReceivedEventHandler NotificationReceived;

        /// <inheritdoc />
        public event NotificationActionTappedEventHandler NotificationActionTapped;

        /// <inheritdoc />
        public void OnNotificationTapped(NotificationEventArgs e)
        {
            NotificationTapped?.Invoke(e);
        }

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

        /// <inheritdoc />
        public bool Cancel(int notificationId)
        {
            try
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0) == false)
                {
                    return false;
                }

                var itemList = new[]
                {
                    notificationId.ToString(CultureInfo.CurrentCulture)
                };

                UNUserNotificationCenter.Current.RemovePendingNotificationRequests(itemList);
                UNUserNotificationCenter.Current.RemoveDeliveredNotifications(itemList);

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        /// <inheritdoc />
        public bool CancelAll()
        {
            try
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0) == false)
                {
                    return false;
                }

                UNUserNotificationCenter.Current.RemoveAllPendingNotificationRequests();
                UNUserNotificationCenter.Current.RemoveAllDeliveredNotifications();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        /// <inheritdoc />
        public Task<bool> Show(Func<NotificationRequestBuilder, NotificationRequest> builder) => Show(builder.Invoke(new NotificationRequestBuilder()));

        /// <inheritdoc />
        public async Task<bool> Show(NotificationRequest request)
        {
            UNNotificationTrigger trigger = null;
            try
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0) == false)
                {
                    return false;
                }

                if (request is null)
                {
                    return false;
                }

                var allowed = await NotificationCenter.AskPermissionAsync().ConfigureAwait(false);
                if (allowed == false)
                {
                    return false;
                }

                var userInfoDictionary = new NSMutableDictionary();
                var dictionary = NotificationCenter.GetRequestSerialize(request);
                foreach (var item in dictionary)
                {
                    userInfoDictionary.SetValueForKey(new NSString(item.Value), new NSString(item.Key));
                }

                using var content = new UNMutableNotificationContent
                {
                    Title = request.Title,
                    Subtitle = request.Subtitle,
                    Body = request.Description,
                    Badge = request.BadgeNumber,
                    UserInfo = userInfoDictionary,
                    Sound = UNNotificationSound.Default,
                };

                // Image Attachment
                if (request.Image != null && request.Image.Length > 0)
                {
                    var nativeImage = await GetNativeImage(request.Image);
                    if (nativeImage != null)
                    {
                        content.Attachments = new[] { nativeImage };
                    }
                }

                if (request.CategoryType != NotificationCategoryType.None)
                {
                    content.CategoryIdentifier = ToNativeCategory(request.CategoryType);
                }
                if (string.IsNullOrWhiteSpace(request.Sound) == false)
                {
                    content.Sound = UNNotificationSound.GetSound(request.Sound);
                }

                var repeats = request.Schedule.RepeatType != NotificationRepeat.No;

                if (repeats && request.Schedule.RepeatType == NotificationRepeat.TimeInterval &&
                    request.Schedule.NotifyRepeatInterval.HasValue)
                {
                    TimeSpan interval = request.Schedule.NotifyRepeatInterval.Value;

                    // Cannot delay and repeat in when TimeInterval
                    trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(interval.TotalSeconds, true);
                }
                else
                {
                    using var notifyTime = GetNsDateComponentsFromDateTime(request);
                    trigger = UNCalendarNotificationTrigger.CreateTrigger(notifyTime, repeats);
                }

                var notificationId =
                    request.NotificationId.ToString(CultureInfo.CurrentCulture);

                var nativeRequest = UNNotificationRequest.FromIdentifier(notificationId, content, trigger);

                await UNUserNotificationCenter.Current.AddNotificationRequestAsync(nativeRequest)
                    .ConfigureAwait(false);

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
            finally
            {
                trigger?.Dispose();
            }
        }

        private async Task<UNNotificationAttachment> GetNativeImage(byte[] imageBytes)
        {
            if (imageBytes is null || imageBytes.Length <= 0)
            {
                return null;
            }

            await using var stream = new MemoryStream(imageBytes);
            var image = Image.FromStream(stream);
            var imageExtension = image.RawFormat.ToString();

            var cache = NSSearchPath.GetDirectories(NSSearchPathDirectory.CachesDirectory,
                NSSearchPathDomain.User);
            var cachesFolder = cache[0];
            var cacheFile = $"{cachesFolder}{NSProcessInfo.ProcessInfo.GloballyUniqueString}.{imageExtension}";

            if (File.Exists(cacheFile))
            {
                File.Delete(cacheFile);
            }
            await File.WriteAllBytesAsync(cacheFile, imageBytes);

            var imageAttachment = NSUrl.CreateFileUrl(cacheFile, false, null);

            var options = new UNNotificationAttachmentOptions();

            return UNNotificationAttachment.FromIdentifier("image", imageAttachment, options, out _);
        }

        private static NSDateComponents GetNsDateComponentsFromDateTime(NotificationRequest notificationRequest)
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

            UNUserNotificationCenter.Current.SetNotificationCategories(new NSSet<UNNotificationCategory>(nativeCategoryList.ToArray()));
        }

        private static UNNotificationCategory RegisterActionList(NotificationCategory category)
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

                var nativeAction = UNNotificationAction.FromIdentifier(notificationAction.ActionId.ToString(CultureInfo.InvariantCulture), notificationAction.Title,
                    ToNativeActionType(notificationAction.iOSAction));
                nativeActionList.Add(nativeAction);
            }

            if (nativeActionList.Any() == false)
            {
                return null;
            }

            var notificationCategory = UNNotificationCategory
                .FromIdentifier(ToNativeCategory(category.CategoryType), nativeActionList.ToArray(), Array.Empty<string>(), UNNotificationCategoryOptions.CustomDismissAction);

            return notificationCategory;
        }

        private static UNNotificationActionOptions ToNativeActionType(iOSActionType type)
        {
            return type switch
            {
                iOSActionType.Foreground => UNNotificationActionOptions.Foreground,
                iOSActionType.Destructive => UNNotificationActionOptions.Destructive,
                iOSActionType.AuthenticationRequired => UNNotificationActionOptions.AuthenticationRequired,
                iOSActionType.None => UNNotificationActionOptions.None,
                _ => UNNotificationActionOptions.None,
            };
        }

        private static string ToNativeCategory(NotificationCategoryType type)
        {
            return type.ToString();
        }
    }
}