<img src="Screenshots/icon.png" alt="icon" width="64px" >

[![Build status](https://ci.appveyor.com/api/projects/status/e02wtx7qx3yf10xa?svg=true)](https://ci.appveyor.com/project/tmt242001/plugin-localnotification) 
![CI](https://github.com/thudugala/Plugin.LocalNotification/workflows/CI/badge.svg?branch=master)
[![NuGet](https://img.shields.io/nuget/v/Plugin.LocalNotification.svg)](https://www.nuget.org/packages/Plugin.LocalNotification/) 
[![NuGet](https://img.shields.io/nuget/dt/Plugin.LocalNotification.svg)](https://www.nuget.org/packages/Plugin.LocalNotification/)

# Plugin.LocalNotification
The local notification plugin provides a way to show local notifications from Xamarin.Forms apps.

# Setup

- `Plugin.LocalNotification` Available on NuGet: https://www.nuget.org/packages/Plugin.LocalNotification
- Install into your platform-specific projects (iOS/Android), and any .NET Standard 2.1 projects required for your app.
- Must Use Xamarin.Forms 4.5.0.356 or above.

## Platform Support

|Platform|Supported|Version|Notes|
| ------------------- | :-----------: | :------------------: | :------------------: |
|Xamarin.iOS|Yes|iOS 10+| |
|Xamarin.Android|Yes|API 19+|Project should [target Android framework 11.0+](https://docs.microsoft.com/en-us/xamarin/android/app-fundamentals/android-api-levels?tabs=vswin#framework)|

# Usage

### Show local notification

```csharp
var notification = new NotificationRequest
{
    NotificationId = 100,
    Title = "Test",
    Description = "Test Description",
    ReturningData = "Dummy data", // Returning data when tapped on notification.
    NotifyTime = DateTime.Now.AddSeconds(30) // Used for Scheduling local notification, if not specified notification will show immediately.
};
NotificationCenter.Current.Show(notification);
```

### Or with Notification Request Builder

```csharp
 NotificationCenter.Current.Show((notification) => notification
                        .NotifyAt(DateTime.Now.AddSeconds(30)) // Used for Scheduling local notification, if not specified notification will show immediately.
                        .WithTitle("Test Title")
                        .WithDescription("Test Description")
                        .WithReturningData("Dummy Data") // Returning data when tapped on notification.
                        .WithNotificationId(100)
                        .Create());
```

### With platform specific options
```csharp
NotificationCenter.Current.Show((notification) => notification
                    .NotifyAt(DateTime.Now.AddSeconds(30))
                    .WithAndroidOptions((android) => android
                         .WithAutoCancel(true)
                         .WithChannelId("General")
                         .WithPriority(NotificationPriority.High)
                         .WithOngoingStatus(true)
                         .Build())
                    .WithiOSOptions((ios) => ios
                        .WithForegroundAlertStatus(true)
                        .WithForegroundSoundStatus(true)
                        .Build())
                    .WithReturningData("Dummy Data")
                    .WithTitle("Test Title")
                    .WithDescription("Test Description")
                    .WithNotificationId(100)
                    .Create());
```

### Receive local notification tap event

```csharp
public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		// Local Notification tap event listener
		NotificationCenter.Current.NotificationTapped += OnLocalNotificationTapped;

		MainPage = new MainPage();
	}
	
	private void OnLocalNotificationTapped(NotificationTappedEventArgs e)
    	{
		// your code goes here
	}
}
```

### Notification received event
*On iOS this event is fired only when the app is in foreground*

```csharp
public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		// Local Notification received event listener
		NotificationCenter.Current.NotificationReceived += OnLocalNotificationReceived;

		MainPage = new MainPage();
	}
	
	private void OnLocalNotificationReceived(NotificationReceivedEventArgs e)
    	{
		// your code goes here
	}
}
```

# Platform Specific Notes

### Android

Project should [target Android framework 11.0+](https://docs.microsoft.com/en-us/xamarin/android/app-fundamentals/android-api-levels?tabs=vswin#framework)

![image](https://user-images.githubusercontent.com/4112014/120728139-5af46400-c530-11eb-9299-fd9ac9221589.png)

#### Setup

To receive Local Notification tap event.
Include the following code in the OnNewIntent() method of MainActivity:

```csharp
public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
{
	protected override void OnCreate(Bundle savedInstanceState)
	{
	        .....
	        // Must create a Notification Channel when API >= 26
                // you can created multiple Notification Channels with different names.
                NotificationCenter.CreateNotificationChannel();		
		.....		
		LoadApplication(new App());
		.....	
		NotificationCenter.NotifyNotificationTapped(Intent);
	}

	protected override void OnNewIntent(Intent intent)
	{
		NotificationCenter.NotifyNotificationTapped(intent);
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

            // Ask the user for permission to show notifications on iOS 10.0+ at startup.
            // If not asked at startup, user will be asked when showing the first notification.
            Plugin.LocalNotification.NotificationCenter.AskPermission();

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }

	public override void WillEnterForeground(UIApplication uiApplication)
        {
            Plugin.LocalNotification.NotificationCenter.ResetApplicationIconBadgeNumber(uiApplication);
        }
}
```
# Screen Record

<img src="Screenshots/screenRecord.gif" alt="Screen Record"  width="512px" >

# Video

[![Local Push Notifications in Xamarin.Forms](https://img.youtube.com/vi/-Nj_TRPlx-8/0.jpg)](https://www.youtube.com/watch?v=-Nj_TRPlx-8)

# Notification Channels

[Setting up Notification Channels](../../wiki/%5BAndroid---=-26%5D-Notification-Channel)

# Custom Sound

[Notification with a Sound-File](../../wiki/Notification-with-a-Sound-File)

# SourceLink Support

In Visual Studio, confirm that SourceLink is enabled. 
Also, Turn off "Just My Code" since, well, this isn't your code.

https://docs.microsoft.com/en-us/dotnet/standard/library-guidance/sourcelink

# Limitations

Only support <b>iOS</b> and <b>Android</b> for the moment. 

# 5.2.0 Documentation

- Please go to [5.2.0 Documentation](../../wiki/Usage-5.2.0), if you are referencing a version below 6.0.0. 
- Version 6.* has setup differences in Android if upgrading from version 5.*

# 4.1.4 Documentation

- Please go to [4.1.4 Documentation](../../wiki/Usage-4.1.4), if you are referencing a version below 5.0.0. 
- Version 5.* has setup differences in Android if upgrading from version 4.*

# 3.0.2 Documentation

- Please go to [3.0.2 Documentation](../../wiki/Usage-3.0.2), if you are referencing a version below 4.0.0. 
- Version 4.* has setup differences in Android if upgrading from version 3.*

# 2.0.7 Documentation

- Please go to [2.0.7 Documentation](../../wiki/Usage-2.0.7), if you are referencing a version below 3.0.0. 
- Version 3.* has breaking changes if upgrading from version 2.*

# Contributing

Contributions are welcome.  Feel free to file issues and pull requests on the repo and they'll be reviewed as time permits.

## Thank you

- Thank you for the Icons by [DinosoftLabs](https://www.iconfinder.com/dinosoftlabs) and [Iconic Hub](https://www.iconfinder.com/iconic_hub) 
- Thank you for the sound file by [Notification sounds](https://notificationsounds.com/notification-sounds/good-things-happen-547)
- Thank you for the tutorial video by [Gerald Versluis](https://www.youtube.com/channel/UCBBZ2kXWmd8eXlHg2wEaClw)
