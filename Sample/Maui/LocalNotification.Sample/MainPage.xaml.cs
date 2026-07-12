using Plugin.LocalNotification;
using Plugin.LocalNotification.Core.Models;
using Plugin.LocalNotification.Core.Models.AndroidOption;
using Plugin.LocalNotification.Core.Models.AppleOption;
using Plugin.LocalNotification.EventArgs;
using System.Globalization;

namespace LocalNotification.Sample;

public partial class MainPage : ContentPage
{
    private const int GeofenceNotificationId = 2001;

    private int _tapCount;
    private readonly INotificationService _notificationService;
    private string _cacheFilePath = string.Empty;

    public MainPage(INotificationService notificationService)
    {
        InitializeComponent();

        _notificationService = notificationService;
        _notificationService.NotificationReceived += ShowCustomAlertFromNotification;
        _notificationService.NotificationActionTapped += Current_NotificationActionTapped;

        NotifyDatePicker.MinimumDate = DateTime.Today;
        NotifyTimePicker.Time = DateTime.Now.TimeOfDay.Add(TimeSpan.FromSeconds(10));

        Appearing += MainPage_Appearing;
    }

    private async void MainPage_Appearing(object? sender, EventArgs e)
    {
        await LoadText();
    }

    private async void ButtonLoadText_Clicked(object? sender, EventArgs e)
    {
        await LoadText();
    }

    private async void ButtonClearText_Clicked(object? sender, EventArgs e)
    {
        await File.WriteAllTextAsync(_cacheFilePath, $"Clear Text {DateTime.Now}");
        TestFileText.Text = await File.ReadAllTextAsync(_cacheFilePath);
    }

    private async void ButtonCancel_Clicked(object? sender, EventArgs e)
    {
        _notificationService.CancelAll();
        await AppendStatusAsync("Cancelled all pending and delivered notifications.");
    }

    private async void ButtonRequestPermissions_Clicked(object? sender, EventArgs e)
    {
        await RequestNotificationPermissionsAsync(requestExactAlarmPermission: true);
        await RequestLocationPermissionAsync();
    }

    private async void Button_Clicked(object? sender, EventArgs e)
    {
        _tapCount++;
        var notificationId = 100 + _tapCount;
        var title = "Local Notification";
        var payload = $"local:{notificationId.ToString(CultureInfo.InvariantCulture)}:{_tapCount.ToString(CultureInfo.InvariantCulture)}";

        var request = new NotificationRequest
        {
            NotificationId = notificationId,
            Title = title,
            Subtitle = $"Tap Count: {_tapCount}",
            Description = UseNotifyTimeSwitch.IsToggled
                ? "This scheduled notification was created from the sample app."
                : "This immediate notification was created from the sample app.",
            BadgeNumber = _tapCount,
            ReturningData = payload,
            CategoryType = NotificationCategoryType.Status,
            Android =
            {
                IconSmallName =
                {
                    ResourceName = "i2"
                },
                Color =
                {
                    ResourceName = "colorPrimary"
                },
                ProgressBar = new AndroidProgressBar
                {
                    IsIndeterminate = false,
                    Max = 20,
                    Progress = _tapCount
                },
                Priority = AndroidPriority.High
            },
            Apple =
            {
                HideForegroundAlert = CustomAlert.IsToggled,
                PlayForegroundSound = ForegroundSound.IsToggled
            }
        };

        if (CustomSoundSwitch.IsToggled)
        {
            request.Sound = DeviceInfo.Platform == DevicePlatform.Android
                ? "good_things_happen"
                : "good_things_happen.aiff";
        }

        if (UseNotifyTimeSwitch.IsToggled)
        {
            var notifyDateTime = (NotifyDatePicker.Date ?? DateTime.Today).Add(NotifyTimePicker.Time ?? TimeSpan.Zero);
            if (notifyDateTime <= DateTime.Now)
            {
                notifyDateTime = DateTime.Now.AddSeconds(10);
            }

            request.Schedule.NotifyAutoCancelTime = DateTimeOffset.Now.AddMinutes(5);
            request.Schedule.NotifyTime = new DateTimeOffset(notifyDateTime);
            request.Schedule.RepeatType = RepeatSwitch.IsToggled ? NotificationRepeat.Daily : NotificationRepeat.No;
        }

        try
        {
            if (!await RequestNotificationPermissionsAsync(UseNotifyTimeSwitch.IsToggled))
            {
                return;
            }

            await _notificationService.Show(request);
            await AppendStatusAsync(UseNotifyTimeSwitch.IsToggled
                ? $"Scheduled local notification {notificationId} for {request.Schedule.NotifyTime:t}."
                : $"Sent local notification {notificationId}.");
        }
        catch (Exception exception)
        {
            await AppendStatusAsync($"Local notification failed: {exception.Message}");
        }
    }

    private async void ButtonUseCurrentLocation_Clicked(object? sender, EventArgs e)
    {
        try
        {
            if (!await RequestLocationPermissionAsync())
            {
                return;
            }

            var location = await Geolocation.Default.GetLocationAsync(new GeolocationRequest(
                GeolocationAccuracy.Medium,
                TimeSpan.FromSeconds(10)));

            if (location is null)
            {
                await AppendStatusAsync("Current location is unavailable.");
                return;
            }

            LatitudeEntry.Text = location.Latitude.ToString(CultureInfo.InvariantCulture);
            LongitudeEntry.Text = location.Longitude.ToString(CultureInfo.InvariantCulture);
            await AppendStatusAsync($"Loaded current location: {LatitudeEntry.Text}, {LongitudeEntry.Text}.");
        }
        catch (Exception exception)
        {
            await AppendStatusAsync($"Unable to get current location: {exception.Message}");
        }
    }

    private async void ButtonCreateGeofence_Clicked(object? sender, EventArgs e)
    {
        try
        {
            if (!await RequestNotificationPermissionsAsync(requestExactAlarmPermission: false) ||
                !await RequestGeofenceLocationPermissionAsync())
            {
                return;
            }

            if (!double.TryParse(LatitudeEntry.Text, CultureInfo.InvariantCulture, out var latitude) ||
                !double.TryParse(LongitudeEntry.Text, CultureInfo.InvariantCulture, out var longitude) ||
                !double.TryParse(RadiusEntry.Text, CultureInfo.InvariantCulture, out var radiusInMeters))
            {
                await DisplayAlertAsync("Location Notification", "Enter valid latitude, longitude, and radius values.", "OK");
                return;
            }

            if (radiusInMeters < 100)
            {
                await DisplayAlertAsync("Location Notification", "Use a radius of at least 100 meters for reliable geofence notifications.", "OK");
                return;
            }

            NotificationRequestGeofence.GeofenceNotifyOn? notifyOn = null;
            if (NotifyOnEntrySwitch.IsToggled)
            {
                notifyOn = NotificationRequestGeofence.GeofenceNotifyOn.OnEntry;
            }

            if (NotifyOnExitSwitch.IsToggled)
            {
                notifyOn = notifyOn.HasValue
                    ? notifyOn.Value | NotificationRequestGeofence.GeofenceNotifyOn.OnExit
                    : NotificationRequestGeofence.GeofenceNotifyOn.OnExit;
            }

            if (!notifyOn.HasValue)
            {
                await DisplayAlertAsync("Location Notification", "Select entry, exit, or both geofence triggers.", "OK");
                return;
            }

            var request = new NotificationRequest
            {
                NotificationId = GeofenceNotificationId,
                Title = "Location Alert",
                Description = "The geofence boundary was crossed.",
                ReturningData = $"geofence:{latitude.ToString(CultureInfo.InvariantCulture)},{longitude.ToString(CultureInfo.InvariantCulture)}",
                Geofence =
                {
                    Center =
                    {
                        Latitude = latitude,
                        Longitude = longitude
                    },
                    RadiusInMeters = radiusInMeters,
                    NotifyOn = notifyOn.Value,
                    Apple =
                    {
                        Repeats = RepeatGeofenceSwitch.IsToggled
                    },
                    Android =
                    {
                        ExpirationDurationInMilliseconds = RepeatGeofenceSwitch.IsToggled ? -1 : 60 * 60 * 1000
                    }
                }
            };

            if (!await EnsureCurrentLocationAvailableAsync())
            {
                return;
            }

            await _notificationService.Show(request);
            await AppendStatusAsync($"Created geofence notification at {latitude}, {longitude} with {radiusInMeters}m radius.");
        }
#if ANDROID
        catch (Android.Gms.Common.Apis.ApiException exception) when (exception.StatusCode == 1000)
        {
            await DisplayAlertAsync(
                "Geofencing unavailable",
                "Android reported GEOFENCE_NOT_AVAILABLE. On the emulator, use a Google Play system image, turn Location on, and set a simulated location in Extended Controls before creating the geofence.",
                "OK");
            await AppendStatusAsync("Android geofencing is unavailable on this emulator/device right now. Set an emulator location or try a Google Play emulator image/physical device.");
        }
#endif
        catch (Exception exception)
        {
            await AppendStatusAsync($"Location notification failed: {exception.Message}");
        }
    }

    private async Task LoadText()
    {
        _cacheFilePath = Path.Combine(FileSystem.Current.CacheDirectory, "testFile.txt");

        if (!File.Exists(_cacheFilePath))
        {
            await File.WriteAllTextAsync(_cacheFilePath, $"Load Text {DateTime.Now}");
        }

        TestFileText.Text = await File.ReadAllTextAsync(_cacheFilePath);
    }

    private async Task AppendStatusAsync(string message)
    {
        if (string.IsNullOrWhiteSpace(_cacheFilePath))
        {
            _cacheFilePath = Path.Combine(FileSystem.Current.CacheDirectory, "testFile.txt");
        }

        var log = $"{DateTime.Now:T} - {message}";
        await File.AppendAllTextAsync(_cacheFilePath, $"{Environment.NewLine}{log}");
        TestFileText.Text = await File.ReadAllTextAsync(_cacheFilePath);
    }

    private async Task<bool> RequestNotificationPermissionsAsync(bool requestExactAlarmPermission)
    {
        var permissionRequest = new NotificationPermission
        {
            Apple =
            {
                NotificationAuthorization = AppleAuthorizationOptions.Alert |
                                            AppleAuthorizationOptions.Badge |
                                            AppleAuthorizationOptions.Sound,
                LocationAuthorization = AppleLocationAuthorization.Always
            },
            Android =
            {
                RequestPermissionToScheduleExactAlarm = requestExactAlarmPermission
            }
        };

        if (await _notificationService.AreNotificationsEnabled())
        {
            await AppendStatusAsync("Notification permissions are already enabled.");
            return true;
        }

        var granted = await _notificationService.RequestNotificationPermission(permissionRequest);
        await AppendStatusAsync(granted
            ? "Notification permissions granted."
            : "Notification permissions were not granted. Enable them in system settings to receive notifications.");

        return granted;
    }

    private async Task<bool> RequestLocationPermissionAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }

        if (status == PermissionStatus.Granted)
        {
            await AppendStatusAsync("Location permission granted.");
            return true;
        }

        await AppendStatusAsync("Location permission was not granted. Enable location permissions in system settings to use geofence notifications.");
        return false;
    }

    private async Task<bool> RequestGeofenceLocationPermissionAsync()
    {
        if (DeviceInfo.Platform != DevicePlatform.Android)
        {
            return await RequestLocationPermissionAsync();
        }

        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }

        if (status == PermissionStatus.Granted)
        {
#if ANDROID
            if (OperatingSystem.IsAndroidVersionAtLeast(29))
            {
                var backgroundStatus = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();
                if (backgroundStatus != PermissionStatus.Granted)
                {
                    backgroundStatus = await Permissions.RequestAsync<Permissions.LocationAlways>();
                }

                if (backgroundStatus != PermissionStatus.Granted)
                {
                    await DisplayAlertAsync("Background location permission required", "Android geofence notifications require Allow all the time location permission. Enable it in system settings, then create the geofence again.", "OK");
                    await AppendStatusAsync("Android background location permission was not granted. Geofence registration cancelled.");
                    return false;
                }
            }

            if (!await EnsureAndroidLocationServicesEnabledAsync())
            {
                return false;
            }
#endif

            await AppendStatusAsync("Android location permission granted for geofence registration.");
            return true;
        }

        await DisplayAlertAsync("Location permission required", "Geofence notifications require location permission.", "OK");
        await AppendStatusAsync("Android location permission was not granted. Geofence registration cancelled.");
        return false;
    }

    private async Task<bool> EnsureAndroidLocationServicesEnabledAsync()
    {
#if ANDROID
        var locationManager = Android.App.Application.Context.GetSystemService(Android.Content.Context.LocationService) as Android.Locations.LocationManager;
        var locationEnabled = OperatingSystem.IsAndroidVersionAtLeast(28)
            ? locationManager?.IsLocationEnabled == true
            : locationManager?.IsProviderEnabled(Android.Locations.LocationManager.GpsProvider) == true ||
              locationManager?.IsProviderEnabled(Android.Locations.LocationManager.NetworkProvider) == true;

        if (locationEnabled)
        {
            return true;
        }

        var openSettings = await DisplayAlertAsync(
            "Location services disabled",
            "Android geofencing is not available while device Location is off. Turn on Location, then create the geofence again.",
            "Open Settings",
            "Cancel");

        if (openSettings)
        {
            var intent = new Android.Content.Intent(Android.Provider.Settings.ActionLocationSourceSettings);
            intent.AddFlags(Android.Content.ActivityFlags.NewTask);
            Android.App.Application.Context.StartActivity(intent);
        }

        await AppendStatusAsync("Android location services are disabled. Geofence registration cancelled.");
        return false;
#else
        return true;
#endif
    }

    private async Task<bool> EnsureCurrentLocationAvailableAsync()
    {
        try
        {
            var location = await Geolocation.Default.GetLocationAsync(new GeolocationRequest(
                GeolocationAccuracy.Medium,
                TimeSpan.FromSeconds(10)));

            if (location is not null)
            {
                return true;
            }
        }
        catch (Exception exception)
        {
            await AppendStatusAsync($"Current location is unavailable: {exception.Message}");
        }

        await DisplayAlertAsync(
            "Current location unavailable",
            "Set a current location on the emulator from Extended Controls > Location, then create the geofence again.",
            "OK");
        return false;
    }

    private void Current_NotificationActionTapped(NotificationActionEventArgs e)
    {
        if (e.IsDismissed)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlertAsync(e.Request.Title, "User dismissed notification.", "OK");
            });
            return;
        }

        if (!e.IsTapped || e.Request is null)
        {
            return;
        }

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await DisplayAlertAsync(e.Request.Title, $"Notification tapped. ReturningData: {e.Request.ReturningData}", "OK");
        });
    }

    private void ShowCustomAlertFromNotification(NotificationEventArgs e)
    {
        if (e.Request is null)
        {
            return;
        }

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            if (!CustomAlert.IsToggled)
            {
                return;
            }

            await DisplayAlertAsync(e.Request.Title, e.Request.Description, "OK");
        });
    }
}
