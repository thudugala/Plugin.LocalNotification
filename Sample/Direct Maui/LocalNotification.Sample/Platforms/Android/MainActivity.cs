using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Plugin.LocalNotification;

namespace LocalNotification.Sample;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Must create a Notification Channel when API >= 26
        // you can created multiple Notification Channel Groups with different names.
        //NotificationCenter.CreateNotificationChannelGroup();

        // Must create a Notification Channel when API >= 26
        // you can created multiple Notification Channels with different names.
        NotificationCenter.CreateNotificationChannel(new Plugin.LocalNotification.Platform.Droid.NotificationChannelRequest
        {
            //Group = AndroidOptions.DefaultGroupId,
            Sound = Resource.Raw.good_things_happen.ToString()
        });

        NotificationCenter.NotifyNotificationTapped(Intent);
    }

    protected override void OnNewIntent(Intent intent)
    {
        NotificationCenter.NotifyNotificationTapped(intent);
        base.OnNewIntent(intent);
    }
}
