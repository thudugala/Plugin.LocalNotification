namespace Plugin.LocalNotification.UnitTests.Tests;

/// <summary>
/// Unit tests for the <see cref="LocalNotificationCenter"/> class.
/// </summary>
public class LocalNotificationCenterTests : IDisposable
{
    /// <summary>
    /// Disposes resources used by the test class. Resets the <see
    /// cref="LocalNotificationCenter.Serializer"/> to a default instance before each test.
    /// </summary>
    public void Dispose()
    {
        LocalNotificationCenter.Serializer = new NotificationSerializer();
    }

    /// <summary>
    /// Tests that <see cref="LocalNotificationCenter.GetRequestListSerialize"/> clears the binary
    /// data of large images when the binary size exceeds the limit.
    /// </summary>
    [Fact]
    public void GetRequestListSerialize_ShouldClearLargeImageBinary_WhenImageBinaryExceedsLimit()
    {
        // Arrange
        var mockSerializer = new Mock<INotificationSerializer>();
        var requestList = new List<NotificationRequest>
        {
            new NotificationRequest
            {
                NotificationId = 1,
                Title = "Test Notification",
                Image = new NotificationImage { Binary = new byte[100000] }
            }
        };
        mockSerializer.Setup(s => s.Serialize(It.IsAny<List<NotificationRequest>>())).Returns("SerializedString");
        LocalNotificationCenter.Serializer = mockSerializer.Object;

        // Act
        var result = LocalNotificationCenter.GetRequestListSerialize(requestList);

        // Assert
        result.Should().Be("SerializedString");
        requestList[0].Image.Binary.Should().BeEmpty();
        mockSerializer.Verify(s => s.Serialize(It.IsAny<List<NotificationRequest>>()), Times.Once);
    }

    /// <summary>
    /// Tests that <see cref="LocalNotificationCenter.GetRequestListSerialize"/> handles cases where
    /// the <see cref="NotificationImage"/> or its binary data is null.
    /// </summary>
    [Fact]
    public void GetRequestListSerialize_ShouldHandleNullNotificationImage()
    {
        // Arrange
        var requestList = new List<NotificationRequest>
        {
            new NotificationRequest
            {
                Image = null // Simulate a null NotificationImage
            },
            new NotificationRequest
            {
                Image = new NotificationImage
                {
                    Binary = null // Simulate a null Binary property
                }
            }
        };

        // Act
        var result = LocalNotificationCenter.GetRequestListSerialize(requestList);

        // Assert
        Assert.NotNull(result); // Ensure serialization does not throw
        Assert.True(result.Contains("[]") || result.Contains("{}")); // Validate serialized output
    }

    /// <summary>
    /// Tests that <see cref="LocalNotificationCenter.GetRequestListSerialize"/> returns a
    /// serialized string when the request list is valid.
    /// </summary>
    [Fact]
    public void GetRequestListSerialize_ShouldReturnSerializedString_WhenRequestListIsValid()
    {
        // Arrange
        var mockSerializer = new Mock<INotificationSerializer>();
        var requestList = new List<NotificationRequest>
        {
            new NotificationRequest
            {
                NotificationId = 1,
                Title = "Test Notification",
                Image = new NotificationImage { Binary = new byte[50000] }
            }
        };
        mockSerializer.Setup(s => s.Serialize(It.IsAny<List<NotificationRequest>>())).Returns("SerializedString");
        LocalNotificationCenter.Serializer = mockSerializer.Object;

        // Act
        var result = LocalNotificationCenter.GetRequestListSerialize(requestList);

        // Assert
        result.Should().Be("SerializedString");
        mockSerializer.Verify(s => s.Serialize(It.IsAny<List<NotificationRequest>>()), Times.Once);
    }
}