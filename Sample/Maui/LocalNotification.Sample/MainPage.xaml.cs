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
                !await RequestLocationPermissionAsync())
            {
                return;
            }

            if (!double.TryParse(LatitudeEntry.Text, CultureInfo.InvariantCulture, out var latitude) ||
                !double.TryParse(LongitudeEntry.Text, CultureInfo.InvariantCulture, out var longitude) ||
                !double.TryParse(RadiusEntry.Text, CultureInfo.InvariantCulture, out var radiusInMeters))
            {
                await DisplayAlert("Location Notification", "Enter valid latitude, longitude, and radius values.", "OK");
                return;
            }

            if (radiusInMeters < 100)
            {
                await DisplayAlert("Location Notification", "Use a radius of at least 100 meters for reliable geofence notifications.", "OK");
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
                await DisplayAlert("Location Notification", "Select entry, exit, or both geofence triggers.", "OK");
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

            await _notificationService.Show(request);
            await AppendStatusAsync($"Created geofence notification at {latitude}, {longitude} with {radiusInMeters}m radius.");
        }
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

    private void Current_NotificationActionTapped(NotificationActionEventArgs e)
    {
        if (e.IsDismissed)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert(e.Request.Title, "User dismissed notification.", "OK");
            });
            return;
        }

        if (!e.IsTapped || e.Request is null)
        {
            return;
        }

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await DisplayAlert(e.Request.Title, $"Notification tapped. ReturningData: {e.Request.ReturningData}", "OK");
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

            await DisplayAlert(e.Request.Title, e.Request.Description, "OK");
        });
    }
}
