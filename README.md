![icon64](https://user-images.githubusercontent.com/4112014/139563161-b7f3cdba-e161-4f6c-80ae-45f0253c4340.png)

![CI](https://github.com/thudugala/Plugin.LocalNotification/workflows/CI/badge.svg?branch=master)
[![NuGet](https://img.shields.io/nuget/v/Plugin.LocalNotification.svg)](https://www.nuget.org/packages/Plugin.LocalNotification/) 
[![NuGet](https://img.shields.io/nuget/dt/Plugin.LocalNotification.svg)](https://www.nuget.org/packages/Plugin.LocalNotification/)

# Plugin.LocalNotification
The local notification plugin provides a way to show local notifications from Xamarin.Forms / .Net MAUI apps.

# Setup

- `Plugin.LocalNotification` Available on NuGet: https://www.nuget.org/packages/Plugin.LocalNotification
- #### .Net MAUI
  - Install Version 10.0.0 above 
  - Install into your project
- #### Xamarin.Forms
  - Install into your platform-specific projects (iOS/Android), and any .NET Standard 2.0/2.1 projects required for your app.

## Platform Support

| Feature                       | Xamarin.iOS | Xamarin.Android | net6.0-ios | net6.0-android |
| ----------------------------- | ----------- | --------------- | ---------- | -------------- |
| Required SDK                  | >= 10       | >= API 19       | >= 10      | >= API 21      |
| Title                         | ✅          | ✅             | ✅         | ✅            | 
| Description                   | ✅          | ✅             | ✅         | ✅            |
| Subtitle                      | ✅          | ✅             | ✅         | ✅            |
| [Schedule](https://github.com/thudugala/Plugin.LocalNotification/wiki/Scheduled-Android-notifications)      | ✅  | ✅   | ✅   | ✅    |
| [Repeat](https://github.com/thudugala/Plugin.LocalNotification/wiki/Repeat-Notification)                    | ✅  | ✅   | ✅   | ✅    |
| [Custom Sounds](https://github.com/thudugala/Plugin.LocalNotification/wiki/Notification-with-a-Sound-File)  | ✅  | ✅   | ✅   | ✅    |
| Images                        | ✅          | ✅             | ✅         | ✅            |
| [Notification Actions](https://github.com/thudugala/Plugin.LocalNotification/wiki/Notification-with-Action) | ✅  | ✅   | ✅   | ✅    |
| Clear Delivered Notifications | ✅          | ✅             | ✅         | ✅            |
| Get Pending Notifications     | ✅          | ✅             | ✅         | ✅            |
| Get Delivered Notifications   | ✅          | ✅             | ✅         | ✅            |

# Usage 

- [Xamarin.Forms](https://github.com/thudugala/Plugin.LocalNotification/wiki/Usage-10.0.0-Xamarin.Forms)
- [.Net MAUI](https://github.com/thudugala/Plugin.LocalNotification/wiki/Usage-10.0.0--.Net-MAUI)

# Screen Record

<img src="https://raw.githubusercontent.com/thudugala/Plugin.LocalNotification/60c9342ba866b1af1278c273f3d41a168901e4ff/Screenshots/screenRecord.gif" alt="Screen Record"  width="512px" >

# Video

[![Local Push Notifications in Xamarin.Forms](https://img.youtube.com/vi/-Nj_TRPlx-8/0.jpg)](https://www.youtube.com/watch?v=-Nj_TRPlx-8)

# SourceLink Support

In Visual Studio, confirm that SourceLink is enabled. 
Also, Turn off "Just My Code" since, well, this isn't your code.

https://docs.microsoft.com/en-us/dotnet/standard/library-guidance/sourcelink

# Limitations

Only support <b>iOS</b> and <b>Android</b> for the moment. 

# Contributing

Contributions are welcome.  Feel free to file issues and pull requests on the repo and they'll be reviewed as time permits.

## Thank you

- Thank you for the Icons by [DinosoftLabs](https://www.iconfinder.com/dinosoftlabs) and [Iconic Hub](https://www.iconfinder.com/iconic_hub) 
- Thank you for the sound file by [Notification sounds](https://notificationsounds.com/notification-sounds/good-things-happen-547)
- Thank you for the tutorial video by [Gerald Versluis](https://www.youtube.com/channel/UCBBZ2kXWmd8eXlHg2wEaClw)
