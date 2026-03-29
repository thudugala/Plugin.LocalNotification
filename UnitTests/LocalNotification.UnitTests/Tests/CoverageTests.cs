using Microsoft.Extensions.Logging;
using Plugin.LocalNotification.AndroidOption;
using Plugin.LocalNotification.Core;
using Plugin.LocalNotification.Core.Models;
using Plugin.LocalNotification.Core.Models.AndroidOption;
using Plugin.LocalNotification.Core.Models.AppleOption;
using Plugin.LocalNotification.EventArgs;
using Plugin.LocalNotification.Platforms;
using System.Reflection;
using System.Threading.Tasks;

namespace Plugin.LocalNotification.UnitTests.Tests;

public class CoverageTests : IDisposable
{
    public void Dispose()
    {
        LocalNotificationCenter.Serializer = new NotificationSerializer();
        LocalNotificationLogger.Logger = null;
        LocalNotificationLogger.LogLevel = LogLevel.Trace;
        GeofenceHandlerRegistry.Handler = null;
        GeofenceHandlerRegistry.ShowNotificationFromSerializedRequest = null;
        LocalNotificationCenter.SetNotificationService(new NotificationServiceImpl());
    }

    [Fact]
    public void CoreModels_ShouldCoverDefaultValues_AndEqualityLogic()
    {
        var notificationAction = new NotificationAction(10) { Title = "Open" };
        notificationAction.ActionId.Should().Be(10);
        notificationAction.Apple.Should().NotBeNull();
        notificationAction.Android.Should().NotBeNull();
        notificationAction.Equals(new NotificationAction(10)).Should().BeTrue();
        notificationAction.Equals(new NotificationAction(11)).Should().BeFalse();
        notificationAction.Equals((NotificationAction?)null).Should().BeFalse();
        notificationAction.Equals((object?)new NotificationAction(10)).Should().BeTrue();
        notificationAction.GetHashCode().Should().Be(10.GetHashCode());

        var notificationCategory = new NotificationCategory(NotificationCategoryType.Alarm) { ActionList = [notificationAction] };
        notificationCategory.ActionList.Should().Contain(notificationAction);
        notificationCategory.Equals(new NotificationCategory(NotificationCategoryType.Alarm)).Should().BeTrue();
        notificationCategory.Equals(new NotificationCategory(NotificationCategoryType.Status)).Should().BeFalse();
        notificationCategory.Equals((NotificationCategory?)null).Should().BeFalse();
        notificationCategory.Equals((object?)new NotificationCategory(NotificationCategoryType.Alarm)).Should().BeTrue();

        var permission = new NotificationPermission();
        permission.AskPermission.Should().BeTrue();
        permission.Android.Should().NotBeNull();
        permission.Apple.Should().NotBeNull();

        var geofence = new NotificationRequestGeofence();
        geofence.IsGeofence.Should().BeFalse();
        geofence.Center = new NotificationRequestGeofence.Position { Latitude = 1.2, Longitude = 3.4 };
        geofence.IsGeofence.Should().BeTrue();
    }

    [Fact]
    public void PlatformOptions_ShouldCoverConstructorsAndFallbackProperties()
    {
        var androidColor = new AndroidColor(7);
        androidColor.Argb.Should().Be(7);
        new AndroidColor("accent").ResourceName.Should().Be("accent");

        var iconWithNameAndType = new AndroidIcon("bell", null);
        iconWithNameAndType.ResourceName.Should().Be("bell");
        iconWithNameAndType.Type.Should().Be(AndroidIcon.DefaultType);
        new AndroidIcon("small").ResourceName.Should().Be("small");
        new AndroidIcon().Type.Should().Be(AndroidIcon.DefaultType);

        var action = new AndroidAction { LaunchAppWhenTapped = true, PendingIntentFlags = AndroidPendingIntentFlags.OneShot };
        action.IconName.Should().NotBeNull();

        new AndroidLaunch().InHighPriority.Should().BeTrue();
        new AndroidNotificationPermission().RequestPermissionToScheduleExactAlarm.Should().BeFalse();

        var androidOptions = new AndroidOptions();
        AndroidOptions.DefaultChannelId.Should().NotBeNullOrWhiteSpace();
        AndroidOptions.DefaultChannelName.Should().NotBeNullOrWhiteSpace();
        AndroidOptions.DefaultGroupId.Should().NotBeNullOrWhiteSpace();
        AndroidOptions.DefaultGroupName.Should().NotBeNullOrWhiteSpace();
        androidOptions.AutoCancel.Should().BeTrue();
        androidOptions.LaunchAppWhenTapped.Should().BeTrue();

        var progress = new AndroidProgressBar { IsIndeterminate = true, Max = 100, Progress = 25 };
        progress.IsIndeterminate.Should().BeTrue();
        progress.Max.Should().Be(100);
        progress.Progress.Should().Be(25);

        var channelGroup = new AndroidNotificationChannelGroupRequest("", "");
        channelGroup.Group.Should().Be(AndroidOptions.DefaultGroupId);
        channelGroup.Name.Should().Be(AndroidOptions.DefaultGroupName);
        channelGroup.Group = "grp";
        channelGroup.Name = "name";
        channelGroup.Group.Should().Be("grp");
        channelGroup.Name.Should().Be("name");

        var channelRequest = new AndroidNotificationChannelRequest
        {
            Id = "",
            Name = ""
        };
        channelRequest.Id.Should().Be(AndroidOptions.DefaultChannelId);
        channelRequest.Name.Should().Be(AndroidOptions.DefaultChannelName);
        channelRequest.Id = "id-1";
        channelRequest.Name = "alerts";
        channelRequest.Id.Should().Be("id-1");
        channelRequest.Name.Should().Be("alerts");

        var iosAction = new AppleAction { Action = AppleActionType.Foreground, Icon = new AppleActionIcon { Type = AppleActionIconType.System, Name = "paperplane" } };
        iosAction.Action.Should().Be(AppleActionType.Foreground);
        iosAction.Icon.Name.Should().Be("paperplane");

        var iosPermission = new AppleNotificationPermission();
        iosPermission.NotificationAuthorization.HasFlag(AppleAuthorizationOptions.Alert).Should().BeTrue();
        iosPermission.LocationAuthorization.Should().Be(AppleLocationAuthorization.No);
    }

    [Fact]
    public void AndroidScheduleOptions_ShouldCoverAllRepeatAndTimeValidationBranches()
    {
        var schedule = new AndroidScheduleOptions { AllowedDelay = TimeSpan.FromMinutes(2) };

        schedule.GetNotifyRepeatInterval(NotificationRepeat.Daily, null).Should().Be(TimeSpan.FromDays(1));
        schedule.GetNotifyRepeatInterval(NotificationRepeat.Weekly, null).Should().Be(TimeSpan.FromDays(7));
        schedule.GetNotifyRepeatInterval(NotificationRepeat.No, null).Should().Be(TimeSpan.Zero);
        schedule.GetNotifyRepeatInterval(NotificationRepeat.TimeInterval, TimeSpan.FromMinutes(5)).Should().Be(TimeSpan.FromMinutes(5));
        schedule.GetNotifyRepeatInterval(NotificationRepeat.TimeInterval, null).Should().Be(TimeSpan.Zero);

        Action invalid = () => schedule.GetNotifyRepeatInterval((NotificationRepeat)999, null);
        invalid.Should().Throw<ArgumentOutOfRangeException>();

        schedule.GetNextNotifyTimeForRepeat(null, NotificationRepeat.Daily, null).Should().BeNull();
        schedule.GetNextNotifyTimeForRepeat(DateTimeOffset.Now, NotificationRepeat.No, null).Should().BeNull();
        schedule.GetNextNotifyTimeForRepeat(DateTimeOffset.Now.AddMinutes(-20), NotificationRepeat.TimeInterval, TimeSpan.FromMinutes(1))
            .Should().BeAfter(DateTimeOffset.Now);

        var now = DateTimeOffset.Now;
        schedule.IsValidNotifyTime(now, now.AddMinutes(-1)).Should().BeTrue();
        schedule.IsValidNotifyTime(now, now.AddMinutes(-10)).Should().BeFalse();
        schedule.IsValidShowNowTime(now, now).Should().BeTrue();
        schedule.IsValidShowNowTime(now, now.AddMinutes(5)).Should().BeFalse();
        schedule.IsValidShowLaterTime(now, now.AddMinutes(2)).Should().BeTrue();
        schedule.IsValidShowLaterTime(now, now).Should().BeFalse();
    }

    [Fact]
    public void NotificationSerializer_ShouldThrowForUnknownTypeInfo()
    {
        var serializer = new NotificationSerializer();

        Action deserializeUnknown = () => serializer.Deserialize<NotificationCategory>("{}");
        Action serializeUnknown = () => serializer.Serialize(new NotificationCategory(NotificationCategoryType.Alarm));

        deserializeUnknown.Should().Throw<InvalidOperationException>();
        serializeUnknown.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void LocalNotificationCenter_ShouldCoverAllSerializationAndDeserializationBranches()
    {
        LocalNotificationCenter.GetRequest(null).Should().NotBeNull();
        LocalNotificationCenter.GetRequest(" ").Should().NotBeNull();
        LocalNotificationCenter.GetRequestList(null).Should().BeEmpty();
        LocalNotificationCenter.GetRequestList(" ").Should().BeEmpty();
        LocalNotificationCenter.GetRequestListSerialize(null!).Should().Be("[]");
        LocalNotificationCenter.GetRequestListSerialize([]).Should().Be("[]");
        LocalNotificationCenter.GetRequestSerialize(null!).Should().Be("[]");

        var request = new NotificationRequest
        {
            NotificationId = 10,
            Title = "title",
            Image = new NotificationImage { Binary = new byte[100001] }
        };
        var serialized = LocalNotificationCenter.GetRequestSerialize(request);
        serialized.Should().NotBeNullOrWhiteSpace();
        request.Image.Binary.Should().BeEmpty();

        var restored = LocalNotificationCenter.GetRequest(serialized);
        restored.NotificationId.Should().Be(10);

        var list = new List<NotificationRequest>
        {
            new() { NotificationId = 1, Image = new NotificationImage { Binary = new byte[100001] } },
            new() { NotificationId = 2 }
        };
        var listSerialized = LocalNotificationCenter.GetRequestListSerialize(list);
        listSerialized.Should().NotBeNullOrWhiteSpace();
        list[0].Image!.Binary.Should().BeEmpty();
        LocalNotificationCenter.GetRequestList(listSerialized).Should().HaveCount(2);

        var serializer = new Mock<INotificationSerializer>();
        serializer.Setup(s => s.Deserialize<NotificationRequest>("request")).Returns((NotificationRequest?)null);
        serializer.Setup(s => s.Deserialize<List<NotificationRequest>>("list")).Returns((List<NotificationRequest>?)null);
        serializer.Setup(s => s.Serialize(It.IsAny<NotificationRequest>())).Returns("request");
        serializer.Setup(s => s.Serialize(It.IsAny<List<NotificationRequest>>())).Returns("list");
        LocalNotificationCenter.Serializer = serializer.Object;

        LocalNotificationCenter.GetRequest("request").Should().NotBeNull();
        LocalNotificationCenter.GetRequestList("list").Should().BeEmpty();
        LocalNotificationCenter.GetRequestSerialize(new NotificationRequest()).Should().Be("request");
        LocalNotificationCenter.GetRequestListSerialize([new NotificationRequest()]).Should().Be("list");
    }

    [Fact]
    public async Task LoggerAndRegistry_ShouldCoverBranches_AndSetters()
    {
        LocalNotificationLogger.Log("no logger path");
        LocalNotificationLogger.Log(new InvalidOperationException("boom"), "no logger path");

        var logger = new Mock<ILogger>();
        logger.Setup(l => l.IsEnabled(LogLevel.Warning)).Returns(true);
        logger.Setup(l => l.IsEnabled(LogLevel.Error)).Returns(true);
        LocalNotificationLogger.Logger = logger.Object;
        LocalNotificationLogger.LogLevel = LogLevel.Warning;

        LocalNotificationLogger.Log("message", "Caller");
        LocalNotificationLogger.Log(new InvalidOperationException("boom"), "error", "Caller");

        logger.Verify(l => l.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception?>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);

        logger.Verify(l => l.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception?>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);

        var flag = false;
        GeofenceHandlerRegistry.ShowNotificationFromSerializedRequest = async _ =>
        {
            await Task.Yield();
            flag = true;
            return true;
        };

        var result = await GeofenceHandlerRegistry.ShowNotificationFromSerializedRequest!("payload");
        result.Should().BeTrue();
        flag.Should().BeTrue();
    }

    [Fact]
    public void BuilderAndEventArgs_ShouldCoverRemainingModelLogic()
    {
        var localBuilder = new LocalNotificationBuilder();
        var category = new NotificationCategory(NotificationCategoryType.Alarm);
        localBuilder.AddCategory(category).Should().BeSameAs(localBuilder);

        localBuilder.SetSerializer(null!).Should().BeSameAs(localBuilder);
        localBuilder.Serializer.Should().NotBeNull();

        var customSerializer = new Mock<INotificationSerializer>().Object;
        localBuilder.SetSerializer(customSerializer);
        localBuilder.Serializer.Should().BeSameAs(customSerializer);

        localBuilder.AddAndroid(android =>
        {
            android.AddChannel(new AndroidNotificationChannelRequest());
            android.AddChannelGroup(new AndroidNotificationChannelGroupRequest());
        }).Should().BeSameAs(localBuilder);

        localBuilder.AddiOS(_ => { }).Should().BeSameAs(localBuilder);

        var androidBuilder = new AndroidLocalNotificationBuilder();
        androidBuilder.AddChannel(new AndroidNotificationChannelRequest()).Should().BeSameAs(androidBuilder);
        androidBuilder.AddChannelGroup(new AndroidNotificationChannelGroupRequest()).Should().BeSameAs(androidBuilder);
        androidBuilder.ChannelRequestList.Should().HaveCount(1);
        androidBuilder.GroupChannelRequestList.Should().HaveCount(1);

        var actionEvent = new NotificationActionEventArgs { ActionId = NotificationActionEventArgs.DismissedActionId };
        actionEvent.IsDismissed.Should().BeTrue();
        actionEvent.IsTapped.Should().BeFalse();
        actionEvent.ActionId = NotificationActionEventArgs.TapActionId;
        actionEvent.IsTapped.Should().BeTrue();

        var receivingEvent = new NotificationEventReceivingArgs { Handled = true };
        receivingEvent.Handled.Should().BeTrue();
        new NotificationEventArgs().Request.Should().NotBeNull();

    }

    [Fact]
    public void NotificationLaunchDetails_ShouldHaveCorrectDefaults()
    {
        var details = new NotificationLaunchDetails
        {
            DidNotificationLaunchApp = false
        };
        details.DidNotificationLaunchApp.Should().BeFalse();
        details.Request.Should().BeNull();
        details.ActionId.Should().BeNull();
    }

    [Fact]
    public void NotificationLaunchDetails_ShouldStoreNotificationData()
    {
        var request = new NotificationRequest
        {
            NotificationId = 42,
            Title = "Launch Test"
        };

        var details = new NotificationLaunchDetails
        {
            DidNotificationLaunchApp = true,
            Request = request,
            ActionId = NotificationActionEventArgs.TapActionId
        };

        details.DidNotificationLaunchApp.Should().BeTrue();
        details.Request.Should().BeSameAs(request);
        details.Request!.NotificationId.Should().Be(42);
        details.ActionId.Should().Be(NotificationActionEventArgs.TapActionId);
    }

    [Fact]
    public void LaunchNotificationDetails_StaticProperty_ShouldBeSettable()
    {
        var original = LocalNotificationCenter.LaunchNotificationDetails;
        try
        {
            LocalNotificationCenter.LaunchNotificationDetails = new NotificationLaunchDetails
            {
                DidNotificationLaunchApp = true,
                Request = new NotificationRequest { NotificationId = 99 },
                ActionId = 100
            };

            LocalNotificationCenter.LaunchNotificationDetails.Should().NotBeNull();
            LocalNotificationCenter.LaunchNotificationDetails!.DidNotificationLaunchApp.Should().BeTrue();
            LocalNotificationCenter.LaunchNotificationDetails.Request!.NotificationId.Should().Be(99);
            LocalNotificationCenter.LaunchNotificationDetails.ActionId.Should().Be(100);
        }
        finally
        {
            LocalNotificationCenter.LaunchNotificationDetails = original;
        }
    }

    [Fact]
    public async Task ServiceImplementation_ShouldCoverGenericImplementationBranches()
    {
        var factoryMethod = typeof(LocalNotificationCenter).GetMethod("CreateNotificationService", BindingFlags.NonPublic | BindingFlags.Static);
        var createdService = factoryMethod!.Invoke(null, null);
        createdService.Should().BeOfType<NotificationServiceImpl>();

        var service = new NotificationServiceImpl();
        service.IsSupported.Should().BeFalse();

        service.NotificationReceiving.Should().BeNull();
        var receivingHandler = new Func<NotificationRequest, Task<NotificationEventReceivingArgs>>(_ => Task.FromResult(new NotificationEventReceivingArgs()));
        service.NotificationReceiving = receivingHandler;
        service.NotificationReceiving.Should().BeSameAs(receivingHandler);

        (await service.AreNotificationsEnabled()).Should().BeFalse();
        (await service.RequestNotificationPermission()).Should().BeFalse();
        (await service.Show(new NotificationRequest())).Should().BeFalse();
        (await service.GetDeliveredNotificationList()).Should().BeEmpty();
        (await service.GetPendingNotificationList()).Should().BeEmpty();

        service.Cancel(1).Should().BeFalse();
        service.CancelAll().Should().BeFalse();
        service.Clear(1).Should().BeFalse();
        service.ClearAll().Should().BeFalse();

        var actionTappedRaised = false;
        var receivedRaised = false;
        var disabledRaised = false;

        service.NotificationActionTapped += _ => actionTappedRaised = true;
        service.NotificationReceived += _ => receivedRaised = true;
        service.NotificationsDisabled += () => disabledRaised = true;

        service.OnNotificationActionTapped(new NotificationActionEventArgs());
        service.OnNotificationReceived(new NotificationEventArgs());
        service.OnNotificationsDisabled();
        service.RegisterCategoryList(new HashSet<NotificationCategory>());

        actionTappedRaised.Should().BeTrue();
        receivedRaised.Should().BeTrue();
        disabledRaised.Should().BeTrue();

        var customService = new Mock<INotificationService>();
        LocalNotificationCenter.SetNotificationService(customService.Object);
        LocalNotificationCenter.Current.Should().BeSameAs(customService.Object);
    }

    [Fact]
    public void AndroidStyles_ShouldCoverModelDefaults()
    {
        var inboxStyle = new AndroidInboxStyle();
        inboxStyle.Lines.Should().NotBeNull().And.BeEmpty();
        inboxStyle.ContentTitle.Should().BeNull();
        inboxStyle.SummaryText.Should().BeNull();
        inboxStyle.Lines.Add("Line 1");
        inboxStyle.Lines.Add("Line 2");
        inboxStyle.Lines.Should().HaveCount(2);

        var person = new AndroidStylePerson { Name = "Alice", Key = "u1", IsBot = false, IsImportant = true };
        person.Name.Should().Be("Alice");
        person.Key.Should().Be("u1");
        person.IsImportant.Should().BeTrue();
        person.IsBot.Should().BeFalse();

        var message = new AndroidStyleMessage { Text = "Hello", Person = person };
        message.Text.Should().Be("Hello");
        message.Person.Should().BeSameAs(person);
        message.Timestamp.Should().BeCloseTo(DateTimeOffset.Now, TimeSpan.FromSeconds(5));

        var messagingStyle = new AndroidMessagingStyle();
        messagingStyle.Person.Should().NotBeNull();
        messagingStyle.Messages.Should().NotBeNull().And.BeEmpty();
        messagingStyle.IsGroupConversation.Should().BeFalse();
        messagingStyle.ConversationTitle.Should().BeNull();
        messagingStyle.ConversationTitle = "Team Chat";
        messagingStyle.ContentTitle.Should().Be("Team Chat");
        messagingStyle.Messages.Add(message);
        messagingStyle.Messages.Should().HaveCount(1);

        var androidOptions = new AndroidOptions();
        androidOptions.Style.Should().BeNull();
        androidOptions.UsesChronometer.Should().BeFalse();
        androidOptions.ChronometerCountDown.Should().BeFalse();
        androidOptions.Colorized.Should().BeFalse();

        androidOptions.Style = inboxStyle;
        androidOptions.Style.Should().BeOfType<AndroidInboxStyle>();
        androidOptions.UsesChronometer = true;
        androidOptions.ChronometerCountDown = true;
        androidOptions.Colorized = true;
        androidOptions.UsesChronometer.Should().BeTrue();
        androidOptions.ChronometerCountDown.Should().BeTrue();
        androidOptions.Colorized.Should().BeTrue();
    }

    [Fact]
    public void NotificationActions_Phase3_ShouldCoverModelDefaults()
    {
        // AndroidActionInput defaults
        var input = new AndroidActionInput();
        input.Label.Should().BeEmpty();
        input.Choices.Should().BeNull();
        input.AllowFreeFormInput.Should().BeTrue();

        // AndroidActionInput with values
        input.Label = "Reply";
        input.AllowFreeFormInput = false;
        input.Choices = ["Yes", "No"];
        input.Label.Should().Be("Reply");
        input.AllowFreeFormInput.Should().BeFalse();
        input.Choices.Should().HaveCount(2);

        // AndroidAction.Inputs list
        var androidAction = new AndroidAction();
        androidAction.Inputs.Should().NotBeNull().And.BeEmpty();
        androidAction.Inputs.Add(input);
        androidAction.Inputs.Should().HaveCount(1);

        // AppleCategoryOptions flags
        var opts = AppleCategoryOptions.CustomDismissAction | AppleCategoryOptions.AllowInCarPlay;
        opts.HasFlag(AppleCategoryOptions.CustomDismissAction).Should().BeTrue();
        opts.HasFlag(AppleCategoryOptions.AllowInCarPlay).Should().BeTrue();
        opts.HasFlag(AppleCategoryOptions.HiddenPreviewsShowTitle).Should().BeFalse();

        // AppleAction text input properties
        var appleAction = new AppleAction();
        appleAction.TextInputButtonTitle.Should().BeNull();
        appleAction.TextInputPlaceholder.Should().BeNull();
        appleAction.TextInputButtonTitle = "Send";
        appleAction.TextInputPlaceholder = "Type a message…";
        appleAction.TextInputButtonTitle.Should().Be("Send");
        appleAction.TextInputPlaceholder.Should().Be("Type a message…");

        // NotificationCategory.AppleOptions
        var category = new NotificationCategory(NotificationCategoryType.Status);
        category.AppleOptions.Should().Be(AppleCategoryOptions.None);
        category.AppleOptions = AppleCategoryOptions.CustomDismissAction;
        category.AppleOptions.Should().Be(AppleCategoryOptions.CustomDismissAction);

        // NotificationActionEventArgs.Input
        var eventArgs = new NotificationActionEventArgs { ActionId = 1 };
        eventArgs.Input.Should().BeNull();
        eventArgs.Input = "Hello from inline reply";
        eventArgs.Input.Should().Be("Hello from inline reply");
    }

    [Fact]
    public void Scheduling_Phase4_ShouldCoverMonthlyRepeatAndScheduleMode()
    {
        // NotificationRepeat.Monthly exists
        var scheduleOptions = new AndroidScheduleOptions();
        scheduleOptions.AlarmType.Should().Be(AndroidAlarmType.RtcWakeup);
        scheduleOptions.ScheduleMode.Should().Be(AndroidScheduleMode.Default);

        // All AndroidScheduleMode values should be distinct
        var modes = Enum.GetValues<AndroidScheduleMode>();
        modes.Should().HaveCountGreaterThan(1);
        modes.Should().Contain(AndroidScheduleMode.Default);
        modes.Should().Contain(AndroidScheduleMode.Inexact);
        modes.Should().Contain(AndroidScheduleMode.InexactAllowWhileIdle);
        modes.Should().Contain(AndroidScheduleMode.Exact);
        modes.Should().Contain(AndroidScheduleMode.ExactAllowWhileIdle);
        modes.Should().Contain(AndroidScheduleMode.AlarmClock);

        // GetNextNotifyTimeForRepeat — Monthly uses calendar arithmetic
        var baseTime = new DateTimeOffset(2025, 1, 31, 9, 0, 0, TimeSpan.Zero);
        var next = scheduleOptions.GetNextNotifyTimeForRepeat(baseTime, NotificationRepeat.Monthly, null);
        // 31 Jan + 1 month = 28 Feb (or 29 in a leap year); DateTimeOffset.AddMonths clamps to last day of month
        next.Should().NotBeNull();
        next!.Value.Month.Should().BeGreaterThan(baseTime.Month);
        next.Value.Hour.Should().Be(9);
        next.Value.Minute.Should().Be(0);

        // GetNextNotifyTimeForRepeat — Monthly: if the next-month date is still in the past, keep adding months
        var veryOldTime = DateTimeOffset.UtcNow.AddMonths(-3);
        var nextFromOld = scheduleOptions.GetNextNotifyTimeForRepeat(veryOldTime, NotificationRepeat.Monthly, null);
        nextFromOld.Should().NotBeNull();
        nextFromOld!.Value.Should().BeAfter(DateTimeOffset.Now);

        // GetNotifyRepeatInterval — Monthly returns Zero (handled via calendar arithmetic)
        var interval = scheduleOptions.GetNotifyRepeatInterval(NotificationRepeat.Monthly, null);
        interval.Should().Be(TimeSpan.Zero);

        // NotificationRepeat enum should contain Monthly
        Enum.IsDefined(typeof(NotificationRepeat), NotificationRepeat.Monthly).Should().BeTrue();

        // ScheduleMode round-trip on AndroidScheduleOptions
        scheduleOptions.ScheduleMode = AndroidScheduleMode.AlarmClock;
        scheduleOptions.ScheduleMode.Should().Be(AndroidScheduleMode.AlarmClock);
    }

    [Fact]
    public void Phase5_Permissions_ShouldCoverNewModelsAndInterface()
    {
        // AndroidOptions.Tag defaults to null
        var options = new AndroidOptions();
        options.Tag.Should().BeNull();

        // Tag round-trip
        options.Tag = "my-tag";
        options.Tag.Should().Be("my-tag");

        // Cancel(int, string?) is cross-platform — generic stub returns same as Cancel(int)
        var svc = new NotificationServiceImpl();
        var cancelResult = svc.Cancel(1, "tag");
        cancelResult.Should().Be(svc.Cancel(1));

        // On the generic (non-Android) platform, LocalNotificationCenter.AndroidService is null
        // because only the Android NotificationServiceImpl implements IAndroidNotificationService
        LocalNotificationCenter.AndroidService.Should().BeNull();

        // IAndroidNotificationService is a subtype of INotificationService
        typeof(IAndroidNotificationService).IsAssignableTo(typeof(INotificationService)).Should().BeTrue();
    }

    [Fact]
    public void Phase6_AppleEnhancements_ShouldCoverNewModels()
    {
        // AppleOptions.CriticalSoundVolume defaults to null
        var apple = new AppleOptions();
        apple.CriticalSoundVolume.Should().BeNull();

        // Round-trip CriticalSoundVolume
        apple.CriticalSoundVolume = 0.75f;
        apple.CriticalSoundVolume.Should().Be(0.75f);

        // AppleOptions.HideThumbnail defaults to null
        apple.HideThumbnail.Should().BeNull();
        apple.HideThumbnail = true;
        apple.HideThumbnail.Should().BeTrue();

        // AppleOptions.ThumbnailClippingRect defaults to null
        apple.ThumbnailClippingRect.Should().BeNull();

        // AppleAttachmentThumbnailClippingRect defaults
        var rect = new AppleAttachmentThumbnailClippingRect();
        rect.X.Should().Be(0.0);
        rect.Y.Should().Be(0.0);
        rect.Width.Should().Be(1.0);
        rect.Height.Should().Be(1.0);

        // Round-trip through AppleOptions
        apple.ThumbnailClippingRect = new AppleAttachmentThumbnailClippingRect { X = 0.1, Y = 0.2, Width = 0.5, Height = 0.6 };
        apple.ThumbnailClippingRect.X.Should().Be(0.1);
        apple.ThumbnailClippingRect.Y.Should().Be(0.2);
        apple.ThumbnailClippingRect.Width.Should().Be(0.5);
        apple.ThumbnailClippingRect.Height.Should().Be(0.6);

        // Serialization round-trip — new fields survive JSON serialization
        var request = new NotificationRequest
        {
            NotificationId = 42,
            Apple = apple
        };
        var json = LocalNotificationCenter.GetRequestSerialize(request);
        var deserialized = LocalNotificationCenter.Serializer.Deserialize<NotificationRequest>(json);
        deserialized.Should().NotBeNull();
        deserialized!.Apple.CriticalSoundVolume.Should().Be(0.75f);
        deserialized.Apple.HideThumbnail.Should().BeTrue();
        deserialized.Apple.ThumbnailClippingRect.Should().NotBeNull();
        deserialized.Apple.ThumbnailClippingRect!.Width.Should().Be(0.5);
    }

    [Fact]
    public void Phase7_AudioAttributesAndLed_ShouldCoverModelDefaults()
    {
        // AndroidAudioAttributeUsage defaults to Notification on AndroidOptions
        var options = new AndroidOptions();
        options.AudioAttributeUsage.Should().Be(AndroidAudioAttributeUsage.Notification);

        // Round-trip AudioAttributeUsage
        options.AudioAttributeUsage = AndroidAudioAttributeUsage.Alarm;
        options.AudioAttributeUsage.Should().Be(AndroidAudioAttributeUsage.Alarm);

        // All enum values are distinct and well-defined
        var usages = Enum.GetValues<AndroidAudioAttributeUsage>();
        usages.Should().Contain(AndroidAudioAttributeUsage.Notification);
        usages.Should().Contain(AndroidAudioAttributeUsage.Alarm);
        usages.Should().Contain(AndroidAudioAttributeUsage.NotificationRingtone);
        usages.Should().Contain(AndroidAudioAttributeUsage.Media);
        usages.Should().Contain(AndroidAudioAttributeUsage.VoiceCommunication);
        usages.Should().Contain(AndroidAudioAttributeUsage.Unknown);

        // LedOnMs and LedOffMs default to null
        options.LedOnMs.Should().BeNull();
        options.LedOffMs.Should().BeNull();

        // Round-trip LED timing
        options.LedOnMs = 500;
        options.LedOffMs = 2000;
        options.LedOnMs.Should().Be(500);
        options.LedOffMs.Should().Be(2000);

        // Serialization round-trip — new fields survive JSON
        var request = new NotificationRequest
        {
            NotificationId = 99,
            Android = options
        };
        var json = LocalNotificationCenter.GetRequestSerialize(request);
        var deserialized = LocalNotificationCenter.Serializer.Deserialize<NotificationRequest>(json);
        deserialized.Should().NotBeNull();
        deserialized!.Android.AudioAttributeUsage.Should().Be(AndroidAudioAttributeUsage.Alarm);
        deserialized.Android.LedOnMs.Should().Be(500);
        deserialized.Android.LedOffMs.Should().Be(2000);
    }

    [Fact]
    public void Phase8_ForegroundService_ShouldCoverModelDefaults()
    {
        // AndroidForegroundServiceType is a [Flags] enum with correct bit values
        ((int)AndroidForegroundServiceType.None).Should().Be(0);
        ((int)AndroidForegroundServiceType.DataSync).Should().Be(1);
        ((int)AndroidForegroundServiceType.MediaPlayback).Should().Be(2);
        ((int)AndroidForegroundServiceType.PhoneCall).Should().Be(4);
        ((int)AndroidForegroundServiceType.Location).Should().Be(8);
        ((int)AndroidForegroundServiceType.ConnectedDevice).Should().Be(16);
        ((int)AndroidForegroundServiceType.MediaProjection).Should().Be(32);
        ((int)AndroidForegroundServiceType.Camera).Should().Be(64);
        ((int)AndroidForegroundServiceType.Microphone).Should().Be(128);
        ((int)AndroidForegroundServiceType.Health).Should().Be(256);
        ((int)AndroidForegroundServiceType.RemoteMessaging).Should().Be(512);
        ((int)AndroidForegroundServiceType.ShortService).Should().Be(2048);

        // Bitwise combination works as expected
        var combined = AndroidForegroundServiceType.DataSync | AndroidForegroundServiceType.Location;
        ((int)combined).Should().Be(9);

        // AndroidForegroundServiceRequest default values
        var req = new AndroidForegroundServiceRequest();
        req.ForegroundServiceType.Should().Be(AndroidForegroundServiceType.ShortService);
        req.Notification.Should().BeNull();

        // Notification can be set and round-trips serialization
        req.Notification = new NotificationRequest
        {
            NotificationId = 42,
            Title = "Background work running",
            Description = "Tap to return to the app.",
        };
        req.Notification.NotificationId.Should().Be(42);
        req.Notification.Title.Should().Be("Background work running");

        // Serialization of the inner NotificationRequest round-trips correctly
        var json = LocalNotificationCenter.GetRequestSerialize(req.Notification);
        var deserialized = LocalNotificationCenter.Serializer.Deserialize<NotificationRequest>(json);
        deserialized.Should().NotBeNull();
        deserialized!.NotificationId.Should().Be(42);
        deserialized.Title.Should().Be("Background work running");

        // IAndroidNotificationService declares the two new methods
        var methods = typeof(IAndroidNotificationService).GetMethods();
        methods.Should().Contain(m => m.Name == nameof(IAndroidNotificationService.StartForegroundServiceAsync));
        methods.Should().Contain(m => m.Name == nameof(IAndroidNotificationService.StopForegroundServiceAsync));
    }

    [Fact]
    public async Task Phase9_GranularPermissionStatus_ShouldCoverModelAndGenericImplementation()
    {
        // Default instance has all false / true for CanScheduleExactAlarms
        var defaultStatus = new NotificationPermissionStatus();
        defaultStatus.IsEnabled.Should().BeFalse();
        defaultStatus.IsAlertEnabled.Should().BeFalse();
        defaultStatus.IsSoundEnabled.Should().BeFalse();
        defaultStatus.IsBadgeEnabled.Should().BeFalse();
        defaultStatus.IsProvisionalEnabled.Should().BeFalse();
        defaultStatus.IsCriticalAlertEnabled.Should().BeFalse();
        defaultStatus.IsCarPlayEnabled.Should().BeFalse();
        defaultStatus.IsTimeSensitiveEnabled.Should().BeFalse();
        defaultStatus.CanScheduleExactAlarms.Should().BeFalse();

        // All properties can be set via init
        var fullStatus = new NotificationPermissionStatus
        {
            IsEnabled = true,
            IsAlertEnabled = true,
            IsSoundEnabled = true,
            IsBadgeEnabled = true,
            IsProvisionalEnabled = true,
            IsCriticalAlertEnabled = true,
            IsCarPlayEnabled = true,
            IsTimeSensitiveEnabled = true,
            CanScheduleExactAlarms = true
        };
        fullStatus.IsEnabled.Should().BeTrue();
        fullStatus.IsAlertEnabled.Should().BeTrue();
        fullStatus.IsSoundEnabled.Should().BeTrue();
        fullStatus.IsBadgeEnabled.Should().BeTrue();
        fullStatus.IsProvisionalEnabled.Should().BeTrue();
        fullStatus.IsCriticalAlertEnabled.Should().BeTrue();
        fullStatus.IsCarPlayEnabled.Should().BeTrue();
        fullStatus.IsTimeSensitiveEnabled.Should().BeTrue();
        fullStatus.CanScheduleExactAlarms.Should().BeTrue();

        // Generic (non-platform) service returns a disabled status with CanScheduleExactAlarms = false
        var genericService = new NotificationServiceImpl();
        var status = await genericService.GetNotificationPermissionStatus();
        status.Should().NotBeNull();
        status.IsEnabled.Should().BeFalse();
        status.CanScheduleExactAlarms.Should().BeFalse();

        // INotificationService interface declares GetNotificationPermissionStatus
        var ifaceMethods = typeof(INotificationService).GetMethods();
        ifaceMethods.Should().Contain(m => m.Name == nameof(INotificationService.GetNotificationPermissionStatus));
    }

    [Fact]
    public void Phase10_NotificationPolicyAccess_ShouldBeDeclaredOnAndroidInterface()
    {
        // IAndroidNotificationService declares both DnD policy methods
        var methods = typeof(IAndroidNotificationService).GetMethods();
        methods.Should().Contain(m => m.Name == "HasNotificationPolicyAccess");
        methods.Should().Contain(m => m.Name == "RequestNotificationPolicyAccess");

        // Both methods return Task<bool>
        var hasAccess = typeof(IAndroidNotificationService).GetMethod("HasNotificationPolicyAccess");
        var requestAccess = typeof(IAndroidNotificationService).GetMethod("RequestNotificationPolicyAccess");
        hasAccess.Should().NotBeNull();
        requestAccess.Should().NotBeNull();
        hasAccess!.ReturnType.Should().Be(typeof(Task<bool>));
        requestAccess!.ReturnType.Should().Be(typeof(Task<bool>));

        // IAndroidNotificationService is still a subtype of INotificationService
        typeof(INotificationService).IsAssignableFrom(typeof(IAndroidNotificationService)).Should().BeTrue();
    }

    [Fact]
    public void Phase11_TickerOnlyAlertOnceSubTextShortcutId_ShouldCoverModelDefaults()
    {
        var options = new AndroidOptions();

        // Ticker defaults to null
        options.Ticker.Should().BeNull();
        options.Ticker = "New message arrived";
        options.Ticker.Should().Be("New message arrived");

        // OnlyAlertOnce defaults to false
        options.OnlyAlertOnce.Should().BeFalse();
        options.OnlyAlertOnce = true;
        options.OnlyAlertOnce.Should().BeTrue();

        // SubText defaults to null
        options.SubText.Should().BeNull();
        options.SubText = "Header line";
        options.SubText.Should().Be("Header line");

        // ShortcutId defaults to null
        options.ShortcutId.Should().BeNull();
        options.ShortcutId = "shortcut.conversation.alice";
        options.ShortcutId.Should().Be("shortcut.conversation.alice");

        // All four survive a serialization round-trip on NotificationRequest
        var request = new NotificationRequest
        {
            NotificationId = 77,
            Android = options
        };
        var json = LocalNotificationCenter.GetRequestSerialize(request);
        var deserialized = LocalNotificationCenter.Serializer.Deserialize<NotificationRequest>(json);
        deserialized.Should().NotBeNull();
        deserialized!.Android.Ticker.Should().Be("New message arrived");
        deserialized.Android.OnlyAlertOnce.Should().BeTrue();
        deserialized.Android.SubText.Should().Be("Header line");
        deserialized.Android.ShortcutId.Should().Be("shortcut.conversation.alice");
    }

    [Fact]
    public async Task Phase12_ActiveNotification_ShouldCoverModelAndGenericImplementation()
    {
        // Default instance has all null/zero properties
        var active = new ActiveNotification();
        active.NotificationId.Should().Be(0);
        active.Title.Should().BeNull();
        active.Body.Should().BeNull();
        active.ChannelId.Should().BeNull();
        active.Tag.Should().BeNull();
        active.GroupKey.Should().BeNull();
        active.BigText.Should().BeNull();
        active.Payload.Should().BeNull();

        // All properties can be set via init
        var full = new ActiveNotification
        {
            NotificationId = 42,
            Title = "Test Title",
            Body = "Test Body",
            ChannelId = "my-channel",
            Tag = "my-tag",
            GroupKey = "my-group",
            BigText = "Expanded big text",
            Payload = "custom-payload"
        };
        full.NotificationId.Should().Be(42);
        full.Title.Should().Be("Test Title");
        full.Body.Should().Be("Test Body");
        full.ChannelId.Should().Be("my-channel");
        full.Tag.Should().Be("my-tag");
        full.GroupKey.Should().Be("my-group");
        full.BigText.Should().Be("Expanded big text");
        full.Payload.Should().Be("custom-payload");

        // Generic (non-platform) service returns an empty list
        var genericService = new NotificationServiceImpl();
        var activeList = await genericService.GetActiveNotifications();
        activeList.Should().NotBeNull();
        activeList.Should().BeEmpty();

        // INotificationService interface declares GetActiveNotifications
        var ifaceMethods = typeof(INotificationService).GetMethods();
        ifaceMethods.Should().Contain(m => m.Name == nameof(INotificationService.GetActiveNotifications));
    }
}
