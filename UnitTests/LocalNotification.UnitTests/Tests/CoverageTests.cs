using Microsoft.Extensions.Logging;
using Plugin.LocalNotification.AndroidOption;
using Plugin.LocalNotification.Core;
using Plugin.LocalNotification.Core.Models;
using Plugin.LocalNotification.Core.Models.AndroidOption;
using Plugin.LocalNotification.Core.Models.iOSOption;
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
        notificationAction.IOS.Should().NotBeNull();
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
        permission.IOS.Should().NotBeNull();

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

        var channelGroup = new NotificationChannelGroupRequest("", "");
        channelGroup.Group.Should().Be(AndroidOptions.DefaultGroupId);
        channelGroup.Name.Should().Be(AndroidOptions.DefaultGroupName);
        channelGroup.Group = "grp";
        channelGroup.Name = "name";
        channelGroup.Group.Should().Be("grp");
        channelGroup.Name.Should().Be("name");

        var channelRequest = new NotificationChannelRequest
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

        var iosAction = new iOSAction { Action = iOSActionType.Foreground, Icon = new iOSActionIcon { Type = iOSActionIconType.System, Name = "paperplane" } };
        iosAction.Action.Should().Be(iOSActionType.Foreground);
        iosAction.Icon.Name.Should().Be("paperplane");

        var iosPermission = new iOSNotificationPermission();
        iosPermission.NotificationAuthorization.HasFlag(iOSAuthorizationOptions.Alert).Should().BeTrue();
        iosPermission.LocationAuthorization.Should().Be(iOSLocationAuthorization.No);
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
        schedule.GetNextNotifyTimeForRepeat(DateTime.Now, NotificationRepeat.No, null).Should().BeNull();
        schedule.GetNextNotifyTimeForRepeat(DateTime.Now.AddMinutes(-20), NotificationRepeat.TimeInterval, TimeSpan.FromMinutes(1))
            .Should().BeAfter(DateTime.Now);

        var now = DateTime.Now;
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
            android.AddChannel(new NotificationChannelRequest());
            android.AddChannelGroup(new NotificationChannelGroupRequest());
        }).Should().BeSameAs(localBuilder);

        localBuilder.AddiOS(_ => { }).Should().BeSameAs(localBuilder);

        var androidBuilder = new AndroidLocalNotificationBuilder();
        androidBuilder.AddChannel(new NotificationChannelRequest()).Should().BeSameAs(androidBuilder);
        androidBuilder.AddChannelGroup(new NotificationChannelGroupRequest()).Should().BeSameAs(androidBuilder);
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
    public async Task ServiceImplementation_ShouldCoverGenericImplementationBranches()
    {
        var factoryMethod = typeof(LocalNotificationCenter).GetMethod("CreateNotificationService", BindingFlags.NonPublic | BindingFlags.Static);
        var createdService = factoryMethod!.Invoke(null, null);
        createdService.Should().BeOfType<NotificationServiceImpl>();

        var service = new NotificationServiceImpl();
        service.IsSupported.Should().BeFalse();

        Action getNotificationReceiving = () => _ = service.NotificationReceiving;
        Action setNotificationReceiving = () => service.NotificationReceiving = _ => Task.FromResult(new NotificationEventReceivingArgs());
        getNotificationReceiving.Should().Throw<NotImplementedException>();
        setNotificationReceiving.Should().Throw<NotImplementedException>();

        await Assert.ThrowsAsync<NotImplementedException>(() => service.AreNotificationsEnabled());
        await Assert.ThrowsAsync<NotImplementedException>(() => service.RequestNotificationPermission());
        await Assert.ThrowsAsync<NotImplementedException>(() => service.Show(new NotificationRequest()));
        await Assert.ThrowsAsync<NotImplementedException>(() => service.GetDeliveredNotificationList());
        await Assert.ThrowsAsync<NotImplementedException>(() => service.GetPendingNotificationList());

        Action[] syncThrows =
        [
            () => service.Cancel(1),
            () => service.CancelAll(),
            () => service.Clear(1),
            () => service.ClearAll(),
            () => service.OnNotificationActionTapped(new NotificationActionEventArgs()),
            () => service.OnNotificationReceived(new NotificationEventArgs()),
            () => service.OnNotificationsDisabled(),
            () => service.RegisterCategoryList(new HashSet<NotificationCategory>())
        ];

        foreach (var action in syncThrows)
        {
            action.Should().Throw<NotImplementedException>();
        }

        var customService = new Mock<INotificationService>();
        LocalNotificationCenter.SetNotificationService(customService.Object);
        LocalNotificationCenter.Current.Should().BeSameAs(customService.Object);
    }
}
