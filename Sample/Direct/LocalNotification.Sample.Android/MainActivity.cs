using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Plugin.LocalNotification;

namespace LocalNotification.Sample.Droid
{
    [Activity(Label = "LocalNotificationRequest.Sample", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            CrossLocalNotificationService.Init();
            LoadApplication(new App());

            CrossLocalNotificationService.NotifyNotificationTapped(this.Intent);
        }

        protected override void OnNewIntent(Intent intent)
        {
            CrossLocalNotificationService.NotifyNotificationTapped(intent);
            base.OnNewIntent(intent);
        }
    }
}