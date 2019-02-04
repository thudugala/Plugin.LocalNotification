using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using Plugin.LocalNotification.Platform.iOS;
using UIKit;
using UserNotifications;
using Xamarin.Forms;

namespace LocalNotification.Sample.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            LocalNotificationService.Init();

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }

        // This method only requires for iOS 8 , 9
        public override void ReceivedLocalNotification(UIApplication application, UILocalNotification notification)
        {
            //Change UIApplicationState to suit different situations
            if (UIApplication.SharedApplication.ApplicationState != UIApplicationState.Active)
            {
                LocalNotificationService.NotifyNotificationTapped(notification);
            }
        }
        
        public override void WillEnterForeground(UIApplication uiApplication)
        {
            //Remove badges on app enter foreground if user cleared the notification in the notification panel 
            UNUserNotificationCenter.Current.GetDeliveredNotifications((notificationList) => {
                if (notificationList.Any())
                {
                    return;
                }
                var appBadges = 0;
                Device.BeginInvokeOnMainThread(() =>
                {
                    UIApplication.SharedApplication.ApplicationIconBadgeNumber = appBadges;
                });
            });
        }
    }
}
