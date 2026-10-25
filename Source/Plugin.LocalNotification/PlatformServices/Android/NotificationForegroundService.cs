using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using Plugin.LocalNotification.Core;
using Plugin.LocalNotification.Core.Models;
using Plugin.LocalNotification.Core.Models.AndroidOption;
using Plugin.LocalNotification.Core.Platforms.Android;
using Plugin.LocalNotification.EventArgs;
using Application = Android.App.Application;

namespace Plugin.LocalNotification.Platforms;

/// <summary>
/// An Android foreground service that displays a persistent notification while background work is running.
/// Start it via <see cref="IAndroidNotificationService.StartForegroundServiceAsync"/> and stop it via
/// <see cref="IAndroidNotificationService.StopForegroundServiceAsync"/>.
/// <para>
/// The consuming app must add the following entries to its <c>AndroidManifest.xml</c>:
/// <list type="bullet">
///   <item><c>&lt;uses-permission android:name="android.permission.FOREGROUND_SERVICE" /&gt;</c></item>
///   <item>Any <c>android.permission.FOREGROUND_SERVICE_*</c> permission required by the chosen <see cref="AndroidForegroundServiceType"/>.</item>
///   <item>
///     A service declaration (use <c>tools:node="merge"</c> to merge with the plugin entry):
///     <code>
///       &lt;service android:name="plugin.LocalNotification.NotificationForegroundService"
///                android:foregroundServiceType="shortService"
///                android:exported="false"
///                tools:node="merge" /&gt;
///     </code>
///   </item>
/// </list>
/// </para>
/// </summary>
[Service(
    Name = ServiceName,
    Enabled = true,
    Exported = false,
    Label = "Plugin LocalNotification Foreground Service")]
internal class NotificationForegroundService : Android.App.Service
{
    /// <summary>The fully-qualified service name registered in the Android manifest.</summary>
    public const string ServiceName = "plugin.LocalNotification." + nameof(NotificationForegroundService);

    internal const string ExtraRequest = ServiceName + ".REQUEST";
    internal const string ExtraForegroundType = ServiceName + ".FOREGROUND_TYPE";
    internal const string ExtraNotificationId = ServiceName + ".NOTIFICATION_ID";

    /// <inheritdoc />
    public override IBinder? OnBind(Intent? intent) => null;

    /// <inheritdoc />
    public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
    {
        try
        {
            var serializedRequest = intent?.GetStringExtra(ExtraRequest);
            var request = LocalNotificationCenter.GetRequest(serializedRequest);
            var foregroundType = intent?.GetIntExtra(ExtraForegroundType, 0) ?? 0;

            if (request is null)
            {
                request = new NotificationRequest
                {
                    NotificationId = intent?.GetIntExtra(ExtraNotificationId, 1000) ?? 1000,
                    Title = Application.Context.PackageManager is null ? "Service" : Application.Context.ApplicationInfo?.LoadLabel(Application.Context.PackageManager) ?? "Service",
                };
            }

            if (string.IsNullOrWhiteSpace(request.Android.ChannelId))
            {
                request.Android.ChannelId = AndroidOptions.DefaultChannelId;
            }

            if (OperatingSystem.IsAndroidVersionAtLeast(26))
            {
                var notifManager = Android.App.NotificationManager.FromContext(this);
                if (notifManager?.GetNotificationChannel(request.Android.ChannelId) is null)
                {
                    LocalNotificationCenter.CreateNotificationChannels(
                    [
                        new AndroidNotificationChannelRequest { Id = request.Android.ChannelId }
                    ]);
                }
            }

            var serialized = LocalNotificationCenter.GetRequestSerialize(request);

            var contentIntent = AndroidUtils.CreateActionIntent(
                request.NotificationId,
                serialized,
                new NotificationAction(NotificationActionEventArgs.TapActionId)
                {
                    Android = { LaunchAppWhenTapped = true }
                },
                typeof(NotificationActionReceiver));

            using var builder = new NotificationCompat.Builder(this, request.Android.ChannelId)?
                .SetContentTitle(request.Title)?
                .SetContentText(request.Description)?
                .SetSmallIcon(ResolveSmallIconId(request.Android.IconSmallName))?
                .SetOngoing(true)?
                .SetAutoCancel(false)?
                .SetContentIntent(contentIntent);

            if (!string.IsNullOrWhiteSpace(request.Subtitle))
            {
                builder?.SetSubText(request.Subtitle);
            }

            var notification = builder?.Build();
            if (notification is null)
            {
                StopSelf();
                return StartCommandResult.NotSticky;
            }

            if (OperatingSystem.IsAndroidVersionAtLeast(29) && foregroundType != 0)
            {
                StartForeground(request.NotificationId, notification, (Android.Content.PM.ForegroundService)foregroundType);
            }
            else
            {
                StartForeground(request.NotificationId, notification);
            }
        }
        catch (Exception ex)
        {
            LocalNotificationLogger.Log(ex);
            StopSelf();
            return StartCommandResult.NotSticky;
        }

        return StartCommandResult.Sticky;
    }

    /// <inheritdoc />
    public override void OnDestroy()
    {
        if (OperatingSystem.IsAndroidVersionAtLeast(29))
        {
            StopForeground(StopForegroundFlags.Remove);
        }       
        base.OnDestroy();
    }

    private static int ResolveSmallIconId(AndroidIcon? icon)
    {
        var iconId = 0;
        if (icon is not null && !string.IsNullOrWhiteSpace(icon.ResourceName))
        {
            var iconType = string.IsNullOrWhiteSpace(icon.Type) ? AndroidIcon.DefaultType : icon.Type;
            iconId = Application.Context.Resources?.GetIdentifier(
                icon.ResourceName, iconType, Application.Context.PackageName) ?? 0;
        }

        if (iconId != 0)
        {
            return iconId;
        }

        iconId = Application.Context.ApplicationInfo?.Icon ?? 0;
        if (iconId == 0)
        {
            iconId = Application.Context.Resources?.GetIdentifier(
                "icon", AndroidIcon.DefaultType, Application.Context.PackageName) ?? 0;
        }

        return iconId;
    }
}
