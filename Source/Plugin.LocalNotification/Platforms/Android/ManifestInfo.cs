﻿//Permissions for android

using Android;
using Android.App;
#if ANDROID
using Microsoft.Maui.Controls.PlatformConfiguration;
#endif

[assembly: UsesPermission(Manifest.Permission.WakeLock)]
[assembly: UsesPermission(Manifest.Permission.ReceiveBootCompleted)]
[assembly: UsesPermission(Manifest.Permission.Vibrate)]
[assembly: UsesPermission(Manifest.Permission.AccessFineLocation)]

#if ANDROID
[assembly: UsesPermission(Manifest.Permission.ScheduleExactAlarm)]
[assembly: System.Reflection.AssemblyMetadata("IsTrimmable", "True")]

#if NET7_0_OR_GREATER
[assembly: UsesPermission(Manifest.Permission.PostNotifications)]
#else
[assembly: UsesPermission("android.permission.POST_NOTIFICATIONS")]
#endif

#elif MONOANDROID
[assembly: UsesPermission("android.permission.SCHEDULE_EXACT_ALARM")]
[assembly: UsesPermission("android.permission.POST_NOTIFICATIONS")]
[assembly: LinkerSafe]
#endif

