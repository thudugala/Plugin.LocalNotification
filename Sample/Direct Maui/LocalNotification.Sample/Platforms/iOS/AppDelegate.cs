using Foundation;
using Plugin.LocalNotification;
using Plugin.LocalNotification.iOSOption;
using UIKit;

namespace LocalNotification.Sample;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
