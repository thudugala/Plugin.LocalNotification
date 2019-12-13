
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Plugin.LocalNotification;

namespace LocalNotification.Sample.Droid
{
    [Activity(Label = "LocalNotification.Sample", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            // Must create a Notification Channel when API >= 26
            // you can created multiple Notification Channels with different names.
            NotificationCenter.CreateNotificationChannel(new Plugin.LocalNotification.Platform.Droid.NotificationChannelRequest
            {
                Sound = Resource.Raw.good_things_happen.ToString()
            });

            LoadApplication(new App());

            NotificationCenter.NotifyNotificationTapped(Intent);
        }

        protected override void OnNewIntent(Intent intent)
        {
            NotificationCenter.NotifyNotificationTapped(intent);
            base.OnNewIntent(intent);
        }
    }
}