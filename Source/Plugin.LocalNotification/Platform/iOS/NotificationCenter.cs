using Foundation;
using Plugin.LocalNotification.Platform.iOS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UIKit;
using UserNotifications;

namespace Plugin.LocalNotification
{
    public static partial class NotificationCenter
    {

        // All identifiers must be unique
        public static Dictionary<string, NotificationAction> NotificationActions { get; } = new Dictionary<string, NotificationAction>();

        static NotificationCenter()
        {
            try
            {
                Current = new Platform.iOS.NotificationServiceImpl();

                UNUserNotificationCenter.Current.Delegate = new LocalNotificationDelegate();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Ask the user for permission to show notifications on iOS 10.0+.
        /// Returns true if Allowed.
        /// </summary>
        public static async void AskPermission()
        {
            await AskPermissionAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Ask the user for permission to show notifications on iOS 10.0+.
        /// Returns true if Allowed.
        /// </summary>
        public static async Task<bool> AskPermissionAsync()
        {
            try
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0) == false)
                {
                    return true;
                }

                var settings = await UNUserNotificationCenter.Current.GetNotificationSettingsAsync().ConfigureAwait(false);
                var allowed = settings.AlertSetting == UNNotificationSetting.Enabled;

                if (allowed)
                {
                    return true;
                }

                // Ask the user for permission to show notifications on iOS 10.0+
                var (alertsAllowed, error) = await UNUserNotificationCenter.Current.RequestAuthorizationAsync(
                                                                               UNAuthorizationOptions.Alert |
                                                                               UNAuthorizationOptions.Badge |
                                                                               UNAuthorizationOptions.Sound)
                                                                           .ConfigureAwait(false);

                Debug.WriteLine(error?.LocalizedDescription);
                return alertsAllowed;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Reset Application Icon Badge Number when there are no notifications.
        /// </summary>
        /// <param name="uiApplication"></param>
        public static void ResetApplicationIconBadgeNumber(UIApplication uiApplication)
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0) == false)
            {
                return;
            }

            //Remove badges on app enter foreground if user cleared the notification in the notification panel
            UNUserNotificationCenter.Current.GetDeliveredNotifications((notificationList) =>
            {
                if (notificationList.Any())
                {
                    return;
                }

                uiApplication.InvokeOnMainThread(() =>
                {
                    uiApplication.ApplicationIconBadgeNumber = 0;
                });
            });
        }

        /// <summary>
        /// Register notification categories and their corresponding actions
        /// </summary>
        public static void RegisterCategories(NotificationCategory[] notificationCategories)
        {
            var categories = new List<UNNotificationCategory>();

            foreach (var category in notificationCategories)
            {
                var notificationCategory = RegisterActions(category);

                categories.Add(notificationCategory);
            }

            UNUserNotificationCenter.Current.SetNotificationCategories(new NSSet<UNNotificationCategory>(categories.ToArray()));
        }

        private static UNNotificationCategory RegisterActions(NotificationCategory category)
        {
            foreach (var notificationAction in category.NotificationActions)
            {
                NotificationActions.Add(notificationAction.Identifier, notificationAction);
            }

            var notificationActions = category
                .NotificationActions
                .Select(t => UNNotificationAction.FromIdentifier(t.Identifier, t.Title, ToNativeActionType(t.ActionType)));

            var notificationCategory = UNNotificationCategory
                .FromIdentifier(category.Identifier, notificationActions.ToArray(), Array.Empty<string>(), UNNotificationCategoryOptions.CustomDismissAction);

            return notificationCategory;
        }

        private static UNNotificationActionOptions ToNativeActionType(ActionTypes actionsType)
        {
            switch(actionsType)
            {
                case ActionTypes.Foreground:
                    return UNNotificationActionOptions.Foreground;

                case ActionTypes.Destructive:
                    return UNNotificationActionOptions.Destructive;

                case ActionTypes.AuthenticationRequired:
                    return UNNotificationActionOptions.AuthenticationRequired;

                default:
                    return UNNotificationActionOptions.None;
            }
        }
    }
}