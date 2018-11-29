<img src="Screenshots/icon.png" alt="icon" width="64px" >

[![Build status](https://ci.appveyor.com/api/projects/status/e02wtx7qx3yf10xa?svg=true)](https://ci.appveyor.com/project/tmt242001/plugin-localnotification)

# Plugin.LocalNotification
The local notification plugin provides a way to show local notifications from Xamarin.Forms apps.

# Setup

- `Plugin.LocalNotification` Available on NuGet: https://www.nuget.org/packages/Plugin.LocalNotification
- Install into your platform-specific projects (iOS/Android), and any .NET Standard 2.0 projects required for your app.

## Platform Support

|Platform|Supported|Version|Notes|
| ------------------- | :-----------: | :------------------: | :------------------: |
|Xamarin.iOS|Yes|iOS 8+| |
|Xamarin.Android|Yes|API 16+|Project should [target Android framework 8.1+](https://docs.microsoft.com/en-us/xamarin/android/app-fundamentals/android-api-levels?tabs=vswin#framework)|

# Usage

### Show local notification

```csharp
var notificationService = DependencyService.Get<ILocalNotificationService>();
var notification = new Plugin.LocalNotification.LocalNotification
{
    NotificationId = 100,
    Title = "Test",
    Description = "Test Description",
    ReturningData = "Dummy data", // Returning data when tapped on notification.
    NotifyTime = DateTime.Now.AddSeconds(30) // Used for Scheduling local notification.
};
notificationService.Show(notification);
```

### Cancel a local notification

```csharp
var notificationService = DependencyService.Get<ILocalNotificationService>();
notificationService.Cancel(100);
```

### Receive local notification tap event

```csharp
public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		// Local Notification tap event listener
		MessagingCenter.Instance.Subscribe<LocalNotificationTappedEvent>(this,
			typeof(LocalNotificationTappedEvent).FullName, OnLocalNotificationTapped);

		MainPage = new MainPage();
	}
	
	private void OnLocalNotificationTapped(LocalNotificationTappedEvent e)
    	{
		// your code goes here
	}
}
```

# Platform Specific Notes

### Android

Notification Icon must be set for notification to appear. 

You can set the notification Icon by setting the following property from inside your Android project:

```csharp
LocalNotificationService.NotificationIconId = Resource.Drawable.YOU_ICON_HERE
```

Scheduled local notifications will persist after device reboot, if permission is set and SDK more than 5.0 Lollipop (API 21)

```XML
<uses-permission android:name="android.permission.RECEIVE_BOOT_COMPLETED" />
```

#### Setup

To receive Local Notification tap event.
Include the following code in the OnNewIntent() method of MainActivity:

```csharp
public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
{
	protected override void OnNewIntent(Intent intent)
	{
		LocalNotificationService.NotifyNotificationTapped(intent);
		base.OnNewIntent(intent);
	}
}
```

### iOS

#### Setup

You must get permission from the user to allow the app to show local notifications.
Also, To receive Local Notification tap event.
Include the following code in the FinishedLaunching() method of AppDelegate:

```csharp
public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
{        
	public override bool FinishedLaunching(UIApplication app, NSDictionary options)
	{
		global::Xamarin.Forms.Forms.Init();

		LocalNotificationService.Init();

		LoadApplication(new App());
		
		.....
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
}
```
# Screen Record

<img src="Screenshots/screenRecord.gif" alt="Screen Record"  width="512px" >

# Limitations

Only support iOS and Android for the moment. 

# Contributing

Contributions are welcome.  Feel free to file issues and pull requests on the repo and they'll be reviewed as time permits.

## Icon

Thank you for the Icon by DinosoftLabs (https://www.iconfinder.com/dinosoftlabs)
