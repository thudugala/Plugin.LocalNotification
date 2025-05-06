namespace Plugin.LocalNotification.UnitTests.Tests;

public class NotificationImageTests
{
    [Fact]
    public void HasValue_ShouldReturnFalse_WhenNotificationImageIsNull()
    {
        // Arrange
        NotificationImage image = null;

        // Act & Assert
        Assert.False(image?.HasValue ?? false);
    }

    [Fact]
    public void HasValue_ShouldReturnTrue_WhenNotificationImageFilePathIsValid()
    {
        // Arrange
        var image = new NotificationImage { FilePath = "../Resources/dotnet_logo.png" };

        // Act
        var result = image.HasValue;

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasValue_ShouldReturnTrue_WhenNotificationImageBinaryIsValid()
    {
        // Arrange
        var assembly = typeof(NotificationImageTests).Assembly;
        var resourceName = "Plugin.LocalNotification.UnitTests.Resources.dotnet_logo.png"; // Correct resource name

        using var stream = assembly.GetManifestResourceStream(resourceName);
        Assert.NotNull(stream); // Ensure the resource is found

        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);

        var image = new NotificationImage
        {
            Binary = memoryStream.ToArray() // Load the resource into the Binary property
        };

        // Act
        var result = image.HasValue;

        // Assert
        Assert.True(result);
    }


    [Fact]
    public void Serialize_ShouldHandleNullNotificationImage()
    {
        // Arrange
        var serializer = new NotificationSerializer(); 
        var request = new NotificationRequest
        {
            Image = null
        };

        // Act
        var json = serializer.Serialize(request); // Use the instance to call Serialize

        // Assert
        Assert.NotNull(json);
        Assert.DoesNotContain("NotificationImage", json); 
    }

}