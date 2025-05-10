namespace Plugin.LocalNotification.AndroidOption;

/// <summary>
/// A grouping of related notification channels
/// </summary>
public class NotificationChannelGroupRequest
{
    private string group = AndroidOptions.DefaultGroupId;
    private string name = AndroidOptions.DefaultGroupName;

    /// <summary>
    /// The id of the group. Must be unique per package. the value may be truncated if it is too long
    /// </summary>
    public string Group
    {
        get => string.IsNullOrWhiteSpace(group) ? AndroidOptions.DefaultGroupId : group;
        set => group = string.IsNullOrWhiteSpace(value) ? AndroidOptions.DefaultGroupId : value;
    }
    /// <summary>
    /// The user visible name of the group, The recommended maximum length is 40 characters; the value may be truncated if it is too long.
    /// </summary>
    public string Name
    {
        get => string.IsNullOrWhiteSpace(name) ? AndroidOptions.DefaultGroupName : name;
        set => name = string.IsNullOrWhiteSpace(value) ? AndroidOptions.DefaultGroupName : value;
    }

    /// <summary>
    /// Constructor to pass values directly
    /// </summary>
    /// <param name="group"></param>
    /// <param name="name"></param>
    public NotificationChannelGroupRequest(string group, string name)
    {
        Group = group;
        Name = name;
    }

    /// <summary>
    /// Default Constructor
    /// </summary>
    public NotificationChannelGroupRequest()
    {
    }
}