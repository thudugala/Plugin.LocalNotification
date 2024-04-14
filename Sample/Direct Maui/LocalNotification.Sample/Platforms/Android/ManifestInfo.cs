//Permissions for android

using Android;
using Android.App;

[assembly: UsesPermission(Manifest.Permission.WakeLock)]
[assembly: UsesPermission(Manifest.Permission.ReceiveBootCompleted)]
[assembly: UsesPermission(Manifest.Permission.Vibrate)]

[assembly: UsesPermission(Manifest.Permission.PostNotifications)]
