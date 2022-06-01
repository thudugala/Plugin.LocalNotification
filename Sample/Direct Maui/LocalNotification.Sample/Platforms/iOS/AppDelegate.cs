using Foundation;
using Plugin.LocalNotification;
using UIKit;

namespace LocalNotification.Sample;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {       
        // if you want to handel push notifications
        //LocalNotificationCenter.Setup(new CustomUserNotificationCenterDelegate());
        LocalNotificationCenter.Setup();

        return base.FinishedLaunching(application, launchOptions);
    }

    public override async void WillEnterForeground(UIApplication application)
    {
        await LocalNotificationCenter.ResetApplicationIconBadgeNumber(application);
        base.WillEnterForeground(application);
    }
}
