using Plugin.LocalNotification.EventArgs;
using System.Globalization;
using UIKit;
using UserNotifications;

namespace Plugin.LocalNotification.Platforms;

/// <summary>
/// Handles iOS user notification center delegate callbacks for local notifications
/// </summary>
/// <inheritdoc />
public class UserNotificationCenterDelegate : UNUserNotificationCenterDelegate
{
    /// <summary>
    /// Called when a notification response is received - when user takes action on a delivered notification
    /// </summary>
    /// <param name="center">The notification center that received the response</param>
    /// <param name="response">The user's response to the notification</param>
    /// <param name="completionHandler">The completion handler to execute when done processing the response</param>
    /// <inheritdoc />
    public override void DidReceiveNotificationResponse(UNUserNotificationCenter center,
        UNNotificationResponse response, Action completionHandler)
    {
        try
        {
            if (response is null)
            {
                return;
            }

            var notificationService = TryGetDefaultIOsNotificationService();
            var notificationRequest = LocalNotificationCenter.GetRequest(response.Notification.Request.Content);

            // if notificationRequest is null this maybe not a notification from this plugin.
            if (notificationRequest is null)
            {
                completionHandler?.Invoke();

                LocalNotificationCenter.Log("Notification request not found");
                return;
            }

            if (response.Notification.Request.Content.Badge != null)
            {
                var badgeNumber = Convert.ToInt32(response.Notification.Request.Content.Badge.ToString(), CultureInfo.CurrentCulture);

                center.InvokeOnMainThread(() =>
                {
                    if (UIDevice.CurrentDevice.CheckSystemVersion(16, 0))
                    {
                        center.SetBadgeCount(badgeNumber, (error) =>
                        {
                            if (error != null)
                            {
                                LocalNotificationCenter.Log(error.LocalizedDescription);
                            }
                        });
                    }
                    else
                    {
                        UIApplication.SharedApplication.ApplicationIconBadgeNumber -= badgeNumber;
                    }
                });
            }

            // Take action based on identifier
            if (!response.IsDefaultAction)
            {
                if (string.IsNullOrWhiteSpace(response.ActionIdentifier) == false &&
                    int.TryParse(response.ActionIdentifier, out var actionId))
                {
                    var actionArgs = new NotificationActionEventArgs
                    {
                        ActionId = actionId,
                        Request = notificationRequest
                    };
                    notificationService.OnNotificationActionTapped(actionArgs);

                    completionHandler?.Invoke();
                    return;
                }
            }

            if (response.IsDismissAction)
            {
                var actionArgs = new NotificationActionEventArgs
                {
                    ActionId = NotificationActionEventArgs.DismissedActionId,
                    Request = notificationRequest
                };
                notificationService.OnNotificationActionTapped(actionArgs);

                completionHandler?.Invoke();
                return;
            }

            var args = new NotificationActionEventArgs
            {
                ActionId = NotificationActionEventArgs.TapActionId,
                Request = notificationRequest
            };
            notificationService.OnNotificationActionTapped(args);

            completionHandler?.Invoke();
        }
        catch (Exception ex)
        {
            LocalNotificationCenter.Log(ex);
        }
    }

    /// <summary>
    /// Called when a notification is about to be presented while the app is in the foreground
    /// </summary>
    /// <param name="center">The notification center that is about to present the notification</param>
    /// <param name="notification">The notification to be presented</param>
    /// <param name="completionHandler">The completion handler to execute with the presentation options</param>
    /// <inheritdoc />
    public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification,
        Action<UNNotificationPresentationOptions> completionHandler)
    {
        try
        {
            var presentationOptions = UNNotificationPresentationOptions.None;

            var notificationService = TryGetDefaultIOsNotificationService();
            var notificationRequest = LocalNotificationCenter.GetRequest(notification?.Request.Content);

            // if notificationRequest is null this maybe not a notification from this plugin.
            if (notificationRequest is null)
            {
                completionHandler?.Invoke(presentationOptions);

                LocalNotificationCenter.Log("Notification request not found");
                return;
            }

            if (notificationRequest.Schedule.NotifyAutoCancelTime.HasValue &&
                notificationRequest.Schedule.NotifyAutoCancelTime <= DateTime.Now)
            {
                _ = notificationService.Cancel(notificationRequest.NotificationId);

                completionHandler?.Invoke(presentationOptions);

                LocalNotificationCenter.Log("Notification Auto Canceled");
                return;
            }

            var requestHandled = false;
            if (notificationService.NotificationReceiving is not null)
            {
                var requestArg = notificationService.NotificationReceiving(notificationRequest).GetAwaiter().GetResult();
                if (requestArg is not null)
                {
                    if (requestArg.Handled)
                    {
                        LocalNotificationCenter.Log("Notification Handled");
                        requestHandled = true;
                    }
                }
            }

            if (requestHandled == false)
            {
                if (OperatingSystem.IsIOSVersionAtLeast(14))
                {
                    if (notificationRequest.iOS.PresentAsBanner)
                    {
                        presentationOptions |= UNNotificationPresentationOptions.Banner;
                    }

                    if (notificationRequest.iOS.ShowInNotificationCenter)
                    {
                        presentationOptions |= UNNotificationPresentationOptions.List;
                    }
                }
                else
                {
                    presentationOptions |= UNNotificationPresentationOptions.Alert;
                }

                if (notificationRequest.iOS.ApplyBadgeValue)
                {
                    presentationOptions |= UNNotificationPresentationOptions.Badge;
                }
                if (notificationRequest.iOS.PlayForegroundSound)
                {
                    presentationOptions |= UNNotificationPresentationOptions.Sound;
                }

                if (notificationRequest.iOS.HideForegroundAlert)
                {
                    presentationOptions = UNNotificationPresentationOptions.None;
                }
            }

            var args = new NotificationEventArgs
            {
                Request = notificationRequest
            };
            notificationService.OnNotificationReceived(args);

            completionHandler?.Invoke(presentationOptions);
        }
        catch (Exception ex)
        {
            LocalNotificationCenter.Log(ex);
        }
    }

    /// <summary>
    /// Attempts to get the default iOS notification service implementation
    /// </summary>
    /// <returns>A new or existing instance of NotificationServiceImpl</returns>
    internal static NotificationServiceImpl TryGetDefaultIOsNotificationService() => LocalNotificationCenter.Current is NotificationServiceImpl notificationService
            ? notificationService
            : new NotificationServiceImpl();
}