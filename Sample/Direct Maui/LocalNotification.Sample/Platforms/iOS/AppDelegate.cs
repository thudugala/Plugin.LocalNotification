using Foundation;
using UIKit;

namespace LocalNotification.Sample;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        // only call this method if you want to handel push notifications
        // Plugin.LocalNotification.NotificationCenter.SetCustomUserNotificationCenterDelegate(new CustomUserNotificationCenterDelegate());

        // Ask the user for permission to show notifications on iOS 10.0+ at startup.
        // If not asked at startup, user will be asked when showing the first notification.
        Plugin.LocalNotification.NotificationCenter.AskPermission();

        return base.FinishedLaunching(application, launchOptions);
    }

    public override async void WillEnterForeground(UIApplication application)
    {
        await Plugin.LocalNotification.NotificationCenter.ResetApplicationIconBadgeNumber(application);
        base.WillEnterForeground(application);
    }
}
