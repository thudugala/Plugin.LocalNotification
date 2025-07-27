namespace Plugin.LocalNotification.AndroidOption;

/// <summary>
/// Represents a grouping of related notification channels for Android notifications.
/// </summary>
public class NotificationChannelGroupRequest
{
    private string group = AndroidOptions.DefaultGroupId;
    private string name = AndroidOptions.DefaultGroupName;

    /// <summary>
    /// Gets or sets the id of the group. Must be unique per package. The value may be truncated if it is too long.
    /// </summary>
    public string Group
    {
        get => string.IsNullOrWhiteSpace(group) ? AndroidOptions.DefaultGroupId : group;
        set => group = string.IsNullOrWhiteSpace(value) ? AndroidOptions.DefaultGroupId : value;
    }
    /// <summary>
    /// Gets or sets the user-visible name of the group. The recommended maximum length is 40 characters; the value may be truncated if it is too long.
    /// </summary>
    public string Name
    {
        get => string.IsNullOrWhiteSpace(name) ? AndroidOptions.DefaultGroupName : name;
        set => name = string.IsNullOrWhiteSpace(value) ? AndroidOptions.DefaultGroupName : value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationChannelGroupRequest"/> class with the specified group id and name.
    /// </summary>
    /// <param name="group">The id of the group.</param>
    /// <param name="name">The user-visible name of the group.</param>
    public NotificationChannelGroupRequest(string group, string name)
    {
        Group = group;
        Name = name;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationChannelGroupRequest"/> class with default values.
    /// </summary>
    public NotificationChannelGroupRequest()
    {
    }
}