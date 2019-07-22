//Permissions for android

using Android;
using Android.App;

[assembly: UsesPermission(Android.Manifest.Permission.ReceiveBootCompleted)]
[assembly: LinkerSafe]