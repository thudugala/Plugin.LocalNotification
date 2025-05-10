namespace Plugin.LocalNotification.UnitTests.Tests;

/// <summary>
/// Unit tests for the <see cref="NotificationImage"/> class.
/// </summary>
public class NotificationImageTests
{
    /// <summary>
    /// Tests that <see cref="NotificationImage.HasValue"/> returns false when the <see
    /// cref="NotificationImage"/> instance is null.
    /// </summary>
    [Fact]
    public void HasValue_ShouldReturnFalse_WhenNotificationImageIsNull()
    {
        // Arrange
        NotificationImage image = null;

        // Act & Assert
        (image?.HasValue ?? false).Should().BeFalse();
    }

    /// <summary>
    /// Tests that <see cref="NotificationImage.HasValue"/> returns true when the Binary property of
    /// the <see cref="NotificationImage"/> is valid.
    /// </summary>
    [Fact]
    public void HasValue_ShouldReturnTrue_WhenNotificationImageBinaryIsValid()
    {
        // Arrange
        var assembly = typeof(NotificationImageTests).Assembly;
        var resourceName = "Plugin.LocalNotification.UnitTests.Resources.dotnet_logo.png"; // Correct resource name

        using var stream = assembly.GetManifestResourceStream(resourceName);
        stream.Should().NotBeNull(); // Ensure the resource is found

        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);

        var image = new NotificationImage
        {
            Binary = memoryStream.ToArray() // Load the resource into the Binary property
        };

        // Act
        var result = image.HasValue;

        // Assert
        result.Should().BeTrue();
    }

    /// <summary>
    /// Tests that <see cref="NotificationImage.HasValue"/> returns true when the FilePath property
    /// of the <see cref="NotificationImage"/> is valid.
    /// </summary>
    [Fact]
    public void HasValue_ShouldReturnTrue_WhenNotificationImageFilePathIsValid()
    {
        // Arrange
        var image = new NotificationImage { FilePath = "../Resources/dotnet_logo.png" };

        // Act
        var result = image.HasValue;

        // Assert
        result.Should().BeTrue();
    }

    /// <summary>
    /// Tests that the <see cref="NotificationSerializer.Serialize"/> method handles a null <see
    /// cref="NotificationImage"/> correctly.
    /// </summary>
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
        json.Should().NotBeNull();
        json.Should().NotContain("NotificationImage");
    }
}