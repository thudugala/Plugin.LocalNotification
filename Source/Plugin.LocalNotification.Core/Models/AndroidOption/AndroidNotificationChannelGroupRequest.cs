namespace Plugin.LocalNotification.Core.Models.AndroidOption;

/// <summary>
/// Represents a grouping of related notification channels for Android notifications.
/// </summary>
public class AndroidNotificationChannelGroupRequest
{

    /// <summary>
    /// Gets or sets the id of the group. Must be unique per package. The value may be truncated if it is too long.
    /// </summary>
    public string Group
    {
        get => string.IsNullOrWhiteSpace(field) ? AndroidOptions.DefaultGroupId : field;
        set => field = string.IsNullOrWhiteSpace(value) ? AndroidOptions.DefaultGroupId : value;
    } = AndroidOptions.DefaultGroupId;
    /// <summary>
    /// Gets or sets the user-visible name of the group. The recommended maximum length is 40 characters; the value may be truncated if it is too long.
    /// </summary>
    public string Name
    {
        get => string.IsNullOrWhiteSpace(field) ? AndroidOptions.DefaultGroupName : field;
        set => field = string.IsNullOrWhiteSpace(value) ? AndroidOptions.DefaultGroupName : value;
    } = AndroidOptions.DefaultGroupName;

    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidNotificationChannelGroupRequest"/> class with the specified group id and name.
    /// </summary>
    /// <param name="group">The id of the group.</param>
    /// <param name="name">The user-visible name of the group.</param>
    public AndroidNotificationChannelGroupRequest(string group, string name)
    {
        Group = group;
        Name = name;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidNotificationChannelGroupRequest"/> class with default values.
    /// </summary>
    public AndroidNotificationChannelGroupRequest()
    {
    }
}