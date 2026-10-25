using Android.App;
using Android.Content;
using Plugin.LocalNotification.Core.Models;
using Application = Android.App.Application;

namespace Plugin.LocalNotification.Core.Platforms.Android;

/// <summary>
/// 
/// </summary>
public class AndroidUtils
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="notificationId"></param>
    /// <param name="serializedRequest"></param>
    /// <param name="action"></param>
    /// <param name="broadcastReceiverType"></param>
    /// <returns></returns>
    public static PendingIntent? CreateActionIntent(int notificationId, string? serializedRequest, NotificationAction action, Type broadcastReceiverType)
    {
        var notificationIntent = action.Android.LaunchAppWhenTapped
            ? (Application.Context.PackageManager?.GetLaunchIntentForPackage(Application.Context.PackageName ??
                                                                          string.Empty))
            : new Intent(Application.Context, broadcastReceiverType);

        notificationIntent?.AddFlags(ActivityFlags.SingleTop)
            .AddFlags(ActivityFlags.IncludeStoppedPackages)
            .PutExtra(RequestConstants.ReturnRequestActionId, action.ActionId)
            .PutExtra(RequestConstants.ReturnRequest, serializedRequest);

        //var requestCode = _random.Next();
        // Cannot be random, then you cannot cancel it.
        var requestCode = notificationId + action.ActionId;

        PendingIntent? pendingIntent = null;
        if (action.Android.LaunchAppWhenTapped)
        {
            pendingIntent = PendingIntent.GetActivity(
                Application.Context,
                requestCode,
                notificationIntent,
                action.Android.PendingIntentFlags.ToNative());
        }
        else if (notificationIntent is not null)
        {
            pendingIntent = PendingIntent.GetBroadcast(
                Application.Context,
                requestCode,
                notificationIntent,
                action.Android.PendingIntentFlags.ToNative()
            );
        }

        return pendingIntent;
    }
}
