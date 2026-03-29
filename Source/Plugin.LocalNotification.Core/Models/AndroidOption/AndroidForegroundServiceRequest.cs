using Plugin.LocalNotification.Core.Models;

namespace Plugin.LocalNotification.Core.Models.AndroidOption;

/// <summary>
/// Describes a foreground service notification to show while background work is running.
/// Pass this to <c>IAndroidNotificationService.StartForegroundServiceAsync</c>
/// to start the foreground service and to
/// <c>IAndroidNotificationService.StopForegroundServiceAsync</c> to stop it.
/// </summary>
public class AndroidForegroundServiceRequest
{
    /// <summary>
    /// The foreground service type declared in <c>AndroidManifest.xml</c>.
    /// Defaults to <see cref="AndroidForegroundServiceType.ShortService"/> which requires only
    /// <c>android.permission.FOREGROUND_SERVICE</c> and is suitable for brief user-triggered tasks.
    /// Change to the type that matches your use case and add the corresponding
    /// <c>android.permission.FOREGROUND_SERVICE_*</c> permission to your manifest.
    /// </summary>
    public AndroidForegroundServiceType ForegroundServiceType { get; set; } = AndroidForegroundServiceType.ShortService;

    /// <summary>
    /// The notification to display while the service is running.
    /// If <see langword="null"/>, a minimal notification is built automatically.
    /// The <see cref="NotificationRequest.NotificationId"/> on this object is used as the foreground notification identifier.
    /// </summary>
    public NotificationRequest? Notification { get; set; }
}
