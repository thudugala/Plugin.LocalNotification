using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;
using Plugin.LocalNotification.EventArgs;
using System.Text.Json;

namespace LocalNotification.Sample;

public partial class MainPage : ContentPage
{
    private int _tapCount;
    private string _cacheFilePath;
    private readonly INotificationService _notificationService;

    public MainPage(INotificationService notificationService)
    {
        InitializeComponent();

        _notificationService = notificationService;

        _notificationService.NotificationReceiving = OnNotificationReceiving;
        _notificationService.NotificationReceived += ShowCustomAlertFromNotification;
        _notificationService.NotificationActionTapped += Current_NotificationActionTapped;

        NotifyDatePicker.MinimumDate = DateTime.Today;
        NotifyTimePicker.Time = DateTime.Now.TimeOfDay.Add(TimeSpan.FromSeconds(10));

        //ScheduleNotificationGroup();
        //ScheduleNotification("first", 10);
        //ScheduleNotification("second", 20);

        _cacheFilePath = FileSystem.Current.CacheDirectory + $"/testFile.txt";

        this.Appearing += MainPage_Appearing;
    }

    private async void MainPage_Appearing(object sender, EventArgs e)
    {
        await LoadText();
    }

    private async void ButtonLoadText_Clicked(object sender, EventArgs e)
    {
        await LoadText();
    }

    private async void ButtonClearText_Clicked(object sender, EventArgs e)
    {
        await File.WriteAllTextAsync(_cacheFilePath, $"Clear Text {DateTime.Now}");
        var fileText = await File.ReadAllTextAsync(_cacheFilePath);
        TestFileText.Text = fileText ?? "No Text";
    }

    private async Task LoadText()
    {
        _cacheFilePath = FileSystem.Current.CacheDirectory + $"/testFile.txt";

        if (!File.Exists(_cacheFilePath))
        {
            await File.WriteAllTextAsync(_cacheFilePath, $"Load Text {DateTime.Now}");
        }

        var fileText = await File.ReadAllTextAsync(_cacheFilePath);

        TestFileText.Text = fileText ?? "No Text";
    }

    private Task<NotificationEventReceivingArgs> OnNotificationReceiving(NotificationRequest request)
    {
        request.Title = $"{request.Title} Modified";

        return Task.FromResult(new NotificationEventReceivingArgs
        {
            Handled = false,
            Request = request
        });
    }

    private void ButtonCancel_Clicked(object sender, EventArgs e)
    {
        _notificationService.CancelAll();
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        var imageStream = GetType().Assembly.GetManifestResourceStream("LocalNotification.Sample.Resources.appicon.png");
        byte[] imageBytes = null;
        if (imageStream != null)
        {
            using (var ms = new MemoryStream())
            {
                await imageStream.CopyToAsync(ms);
                imageBytes = ms.ToArray();
            }
        }

        _tapCount++;
        var notificationId = 100 + _tapCount;
        var title = "Test";
        var list = new List<string>
            {
                typeof(NotificationPage).FullName,
                notificationId.ToString(),
                title,
                _tapCount.ToString()
            };
        // No need to use NotificationSerializer, you can use your own one.
        var serializeReturningData = JsonSerializer.Serialize(list);

        var request = new NotificationRequest
        {
            NotificationId = notificationId,
            Title = title,
            Subtitle = $"Tap Count: {_tapCount}",
            Description = $"Tap Count: {_tapCount}",
            BadgeNumber = _tapCount,
            ReturningData = serializeReturningData,
            Image =
                {
                    //ResourceName = "icon",
                    Binary = imageBytes
                },
            CategoryType = NotificationCategoryType.Status,
            Android =
                {
                    IconSmallName =
                    {
                        ResourceName = "i2",
                    },
                    Color =
                    {
                        ResourceName = "colorPrimary"
                    },
                    IsProgressBarIndeterminate = false,
                    ProgressBarMax = 20,
                    ProgressBarProgress = _tapCount,
                    Priority = AndroidPriority.High
                    //AutoCancel = false,
                    //Ongoing = true
                },
            iOS =
                {
                    HideForegroundAlert = CustomAlert.IsToggled,
                    PlayForegroundSound = ForegroundSound.IsToggled
                }
        };

        // if not specified, default sound will be played.
        if (CustomSoundSwitch.IsToggled)
        {
            request.Sound = DeviceInfo.Platform == DevicePlatform.Android
                ? "good_things_happen"
                : "good_things_happen.aiff";
        }

        // if not specified, notification will show immediately.
        if (UseNotifyTimeSwitch.IsToggled)
        {
            var notifyDateTime = NotifyDatePicker.Date.Add(NotifyTimePicker.Time);
            if (notifyDateTime <= DateTime.Now)
            {
                notifyDateTime = DateTime.Now.AddSeconds(10);
            }
            //var notifyDateTime = DateTime.Now.AddSeconds(30);

            request.Schedule.NotifyAutoCancelTime = DateTime.Now.AddMinutes(5);
            request.Schedule.NotifyTime = notifyDateTime;
            //request.Schedule.RepeatType = RepeatSwitch.IsToggled ? NotificationRepeat.Daily : NotificationRepeat.No;
            //request.Schedule.RepeatType = NotificationRepeat.TimeInterval;
            //request.Schedule.NotifyRepeatInterval = TimeSpan.FromMinutes(2);
        }

        try
        {
            if (await _notificationService.AreNotificationsEnabled() == false)
            {
                await _notificationService.RequestNotificationPermission();
            }

            var ff = await _notificationService.Show(request);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
    }

    private async void Current_NotificationActionTapped(NotificationActionEventArgs e)
    {
        await File.AppendAllTextAsync(_cacheFilePath, $"{Environment.NewLine}ActionId {e.ActionId} {DateTime.Now}");

        if (e.IsDismissed)
        {
            await File.AppendAllTextAsync(_cacheFilePath, $"{Environment.NewLine}Dismissed {DateTime.Now}");
            MainThread.BeginInvokeOnMainThread(() =>
            {
                DisplayAlert(e.Request.Title, "User Dismissed Notification", "OK");
            });
            return;
        }

        if (e.IsTapped)
        {
            await File.AppendAllTextAsync(_cacheFilePath, $"{Environment.NewLine}Tapped {DateTime.Now}");
            if (e.Request is null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert(e.Request.Title, $"No Request", "OK");
                });
                return;
            }

            // No need to use NotificationSerializer, you can use your own one.
            var list = JsonSerializer.Deserialize<List<string>>(e.Request.ReturningData);
            if (list is null || list.Count != 4)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert(e.Request.Title, $"No ReturningData {e.Request.ReturningData}", "OK");
                });
                return;
            }

            if (list[0] != typeof(NotificationPage).FullName)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert(e.Request.Title, $"Not NotificationPage", "OK");
                });
                return;
            }

            var id = list[1];
            var message = list[2];
            var tapCount = list[3];

            await ((NavigationPage)App.Current.MainPage).Navigation.PushModalAsync(
                new NotificationPage(_notificationService,
                int.Parse(id), 
                message,
                int.Parse(tapCount)));
            return;
        }

        switch (e.ActionId)
        {
            case 100:
                await File.AppendAllTextAsync(_cacheFilePath, $"{Environment.NewLine}Hello {DateTime.Now}");

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert(e.Request.Title, "Hello Button was Tapped", "OK");
                });

                _notificationService.Cancel(e.Request.NotificationId);
                break;

            case 101:
                await File.AppendAllTextAsync(_cacheFilePath, $"{Environment.NewLine}Cancel {DateTime.Now}");
                _notificationService.Cancel(e.Request.NotificationId);
                break;
        }
    }

    private void ScheduleNotification(string title, double seconds)
    {
        _tapCount++;
        var notificationId = (int)DateTime.Now.Ticks;
        var list = new List<string>
            {
                typeof(NotificationPage).FullName,
                notificationId.ToString(),
                title,
                _tapCount.ToString()
            };
        var serializeReturningData = JsonSerializer.Serialize(list);

        var notification = new NotificationRequest
        {
            NotificationId = notificationId,
            Title = title,
            Description = $"Tap Count: {_tapCount}",
            ReturningData = serializeReturningData,
            Group = AndroidOptions.DefaultGroupId,
            Schedule =
                {
                    NotifyTime = DateTime.Now.AddSeconds(seconds),
                    RepeatType = NotificationRepeat.TimeInterval,
                    NotifyRepeatInterval = TimeSpan.FromSeconds(10),
                }
        };
        _notificationService.Show(notification);
    }

    private void ScheduleNotificationGroup()
    {
        var notificationId = (int)DateTime.Now.Ticks;
        var notification = new NotificationRequest
        {
            NotificationId = notificationId,
            Title = "Summary",
            Description = "Summary Desc",
            Group = AndroidOptions.DefaultGroupId,
            Android =
                {
                    IsGroupSummary = true
                }
        };
        _notificationService.Show(notification);
    }

    private void ShowCustomAlertFromNotification(NotificationEventArgs e)
    {
        if (e.Request is null)
        {
            return;
        }

        System.Diagnostics.Debug.WriteLine(e);

        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (!CustomAlert.IsToggled)
            {
                return;
            }
            var requestJson = JsonSerializer.Serialize(e.Request);

            DisplayAlert(e.Request.Title, requestJson, "OK");
        });
    }
}

