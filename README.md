![icon64](https://user-images.githubusercontent.com/4112014/139563161-b7f3cdba-e161-4f6c-80ae-45f0253c4340.png)

[![CI](https://github.com/thudugala/Plugin.LocalNotification/actions/workflows/main.yml/badge.svg)](https://github.com/thudugala/Plugin.LocalNotification/actions/workflows/main.yml)
[![Nuget Build](https://github.com/thudugala/Plugin.LocalNotification/actions/workflows/nuget.yml/badge.svg)](https://github.com/thudugala/Plugin.LocalNotification/actions/workflows/nuget.yml)

# Plugin.LocalNotification
The local notification plugin provides a way to display local notifications in .NET MAUI apps.

# Setup

- `Plugin.LocalNotification` Available on
  - NuGet: https://www.nuget.org/packages/Plugin.LocalNotification [![NuGet](https://img.shields.io/nuget/v/Plugin.LocalNotification.svg)](https://www.nuget.org/packages/Plugin.LocalNotification/) [![NuGet](https://img.shields.io/nuget/dt/Plugin.LocalNotification.svg)](https://www.nuget.org/packages/Plugin.LocalNotification/)
  - NuGet: https://www.nuget.org/packages/Plugin.LocalNotification.Geofence [![NuGet](https://img.shields.io/nuget/v/Plugin.LocalNotification.Geofence.svg)](https://www.nuget.org/packages/Plugin.LocalNotification.Geofence/) [![NuGet](https://img.shields.io/nuget/dt/Plugin.LocalNotification.Geofence.svg)](https://www.nuget.org/packages/Plugin.LocalNotification.Geofence/)
  - NuGet: https://www.nuget.org/packages/Plugin.LocalNotification.Core [![NuGet](https://img.shields.io/nuget/v/Plugin.LocalNotification.Core.svg)](https://www.nuget.org/packages/Plugin.LocalNotification.Core/) [![NuGet](https://img.shields.io/nuget/dt/Plugin.LocalNotification.Core.svg)](https://www.nuget.org/packages/Plugin.LocalNotification.Core/)
- #### .Net MAUI
  - Install Version 10.0.0 above 
  - Install into your project
- #### Xamarin.Forms (Support ended on May 1, 2024)
  - Install Version 11.0.0 below 
  - Install into your platform-specific projects (iOS/Android), and any .NET Standard 2.0/2.1 projects required for your app.

## Platform Support

| Feature                       | net10.0-ios  | net10.0-android  |
| ----------------------------- | ----------- | --------------- |
| Build SDK                     | >= 15              | >= API 36              |
| Supported OS Version          | >= 15              | >= API 21              |
| Title                         | ✅                | ✅                     |
| Description                   | ✅                | ✅                     |
| Subtitle                      | ✅                | ✅                     |
| [Schedule](https://github.com/thudugala/Plugin.LocalNotification/wiki/3.-Scheduled-Android-notifications)      | ✅ | ✅ |
| [Repeat](https://github.com/thudugala/Plugin.LocalNotification/wiki/4.-Repeat-Notification)                    | ✅ | ✅ |
| [Custom Sounds](https://github.com/thudugala/Plugin.LocalNotification/wiki/Notification-with-a-Sound-File)     | ✅ | ✅ |
| Images                        | ✅                | ✅                     |
| [Notification Actions](https://github.com/thudugala/Plugin.LocalNotification/wiki/5.-Notification-with-Action) | ✅ | ✅ |
| Clear Delivered Notifications | ✅                | ✅                     |
| Get Pending Notifications     | ✅                | ✅                     |
| Get Delivered Notifications   | ✅                | ✅                     |
| [Location Notifications](https://github.com/thudugala/Plugin.LocalNotification/wiki/Location-Notifications)    | ✅ | ✅ |
| App Launch Detection          | ✅                | ✅                     |
| [Notification Channels](https://github.com/thudugala/Plugin.LocalNotification/wiki/[Android---=-26]-Notification-Channel) | ❌ | ✅ |
| [Notification Styles](https://github.com/thudugala/Plugin.LocalNotification/wiki/[Android]-Notification-Styles) (Inbox, Messaging, BigText, BigPicture) | ❌ | ✅ |
| Chronometer / Timer Display   | ❌                | ✅                     |
| Colorized Notifications       | ❌                | ✅                     |
| Audio Attribute (sound routing control) | ❌     | ✅                     |
| LED Lighting (colour + blink timing) | ❌        | ✅                     |
| [Foreground Service](https://github.com/thudugala/Plugin.LocalNotification/wiki/[Android]-Foreground-Service) (persistent background-work notification) | ❌ | ✅ |

# Usage

- [.Net MAUI](https://github.com/thudugala/Plugin.LocalNotification/wiki/1.-Usage-10.0.0--.Net-MAUI)

## Android-Specific Features

| Guide | Description |
|-------|-------------|
| [Notification Channels](https://github.com/thudugala/Plugin.LocalNotification/wiki/[Android---=-26]-Notification-Channel) | Create and manage notification channels (API 26+) |
| [Notification Styles](https://github.com/thudugala/Plugin.LocalNotification/wiki/[Android]-Notification-Styles) | Inbox, Messaging, Chronometer, Colorized, Audio Attributes, LED |
| [Notification Groups](https://github.com/thudugala/Plugin.LocalNotification/wiki/[Android]-How-to-group-notifications) | Group related notifications under a summary |
| [Icon Customisation](https://github.com/thudugala/Plugin.LocalNotification/wiki/[Android]-Icon-customisation) | Custom small and large notification icons |
| [Foreground Service](https://github.com/thudugala/Plugin.LocalNotification/wiki/[Android]-Foreground-Service) | Persistent notification while background work is running |

# Screen Record

<img src="https://raw.githubusercontent.com/thudugala/Plugin.LocalNotification/60c9342ba866b1af1278c273f3d41a168901e4ff/Screenshots/screenRecord.gif" alt="Screen Record"  width="512px" >

# Video

### .Net MAUI
[![Local Push Notifications in .Net MAUI](https://img.youtube.com/vi/dWdXXGa1_hI/0.jpg)](https://www.youtube.com/watch?v=dWdXXGa1_hI)

### Xamarin.Forms (Support ended on May 1, 2024)
[![Local Push Notifications in Xamarin.Forms](https://img.youtube.com/vi/-Nj_TRPlx-8/0.jpg)](https://www.youtube.com/watch?v=-Nj_TRPlx-8)

# SourceLink Support

In Visual Studio, confirm that SourceLink is enabled. 
Also, turn off "Just My Code" since, well, this isn't your code.

https://docs.microsoft.com/en-us/dotnet/standard/library-guidance/sourcelink

# Limitations

Only support <b>iOS</b> and <b>Android</b> for the moment. 

# Contributing

Contributions are welcome.  Feel free to file issues and pull requests on the repo and they'll be reviewed as time permits.

## Thank you

- Thank you for the Icons by [DinosoftLabs](https://www.iconfinder.com/dinosoftlabs) and [Iconic Hub](https://www.iconfinder.com/iconic_hub) 
- Thank you for the sound file by [Notification sounds](https://notificationsounds.com/notification-sounds/good-things-happen-547)
- Thank you for the tutorial video by [Gerald Versluis](https://www.youtube.com/channel/UCBBZ2kXWmd8eXlHg2wEaClw)
