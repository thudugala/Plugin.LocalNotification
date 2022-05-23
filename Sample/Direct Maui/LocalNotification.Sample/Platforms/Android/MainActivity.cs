using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Plugin.LocalNotification;
using Plugin.LocalNotification.Platforms.Android;

namespace LocalNotification.Sample;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Must create a Notification Channel when API >= 26
        // you can created multiple Notification Channel Groups with different names.
        //LocalNotificationCenter.CreateNotificationChannelGroup();

        // Must create a Notification Channel when API >= 26
        // you can created multiple Notification Channels with different names.
        LocalNotificationCenter.CreateNotificationChannel(new NotificationChannelRequest
        {
            //Group = AndroidOptions.DefaultGroupId,
            Sound = Resource.Raw.good_things_happen.ToString()
        });

        LocalNotificationCenter.NotifyNotificationTapped(Intent);
    }

    protected override void OnNewIntent(Intent intent)
    {
        LocalNotificationCenter.NotifyNotificationTapped(intent);
        base.OnNewIntent(intent);
    }
}
