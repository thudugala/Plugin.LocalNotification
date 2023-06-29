
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;

namespace LocalNotification.Sample.Droid
{
    [Activity(Label = "LocalNotification.Sample", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {            
            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            LocalNotificationCenter.MainActivity = this;

            // Must create a Notification Channel when API >= 26
            // you can created multiple Notification Channels with different names.
            // you can created multiple Notification Channel Groups with different names.
            LocalNotificationCenter.CreateNotificationChannel(new NotificationChannelRequest
            {
                //Group = AndroidOptions.DefaultGroupId,
                Sound = Resource.Raw.good_things_happen.ToString()
            });
            
            LoadApplication(new App());

            LocalNotificationCenter.NotifyNotificationTapped(Intent);
        }

        protected override void OnNewIntent(Intent intent)
        {
            LocalNotificationCenter.NotifyNotificationTapped(intent);
            base.OnNewIntent(intent);
        }
    }
}