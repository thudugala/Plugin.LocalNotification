namespace Plugin.LocalNotification.UnitTests.Tests;

/// <summary>
/// Unit tests for the <see cref="NotificationRequestGeofence.Position"/> class serialization and deserialization.
/// </summary>
public class GeofencePositionSerializationTests
{
    /// <summary>
    /// Tests that a NotificationRequest without geofence can be serialized and deserialized correctly.
    /// This reproduces the bug where Geofence.Center.Latitude and Longitude are NaN and cause deserialization errors.
    /// </summary>
    [Fact]
    public void Serialize_Deserialize_ShouldHandle_NotificationRequest_WithoutGeofence()
    {
        // Arrange
        var serializer = new NotificationSerializer();
        var originalRequest = new NotificationRequest
        {
            NotificationId = 1,
            Title = "Test Notification",
            Description = "This is a test notification without geofence",
            // Geofence is not set, so Center.Latitude and Center.Longitude will be NaN
        };

        // Act
        var json = serializer.Serialize(originalRequest);
        var deserializedRequest = serializer.Deserialize<NotificationRequest>(json);

        // Assert
        deserializedRequest.Should().NotBeNull();
        deserializedRequest.NotificationId.Should().Be(1);
        deserializedRequest.Title.Should().Be("Test Notification");
        deserializedRequest.Description.Should().Be("This is a test notification without geofence");
        deserializedRequest.Geofence.Should().NotBeNull();
        deserializedRequest.Geofence.Center.Should().NotBeNull();
        deserializedRequest.Geofence.Center.Latitude.Should().Be(double.NaN);
        deserializedRequest.Geofence.Center.Longitude.Should().Be(double.NaN);
        deserializedRequest.Geofence.IsGeofence.Should().BeFalse();
    }

    /// <summary>
    /// Tests that a NotificationRequest with valid geofence coordinates can be serialized and deserialized correctly.
    /// </summary>
    [Fact]
    public void Serialize_Deserialize_ShouldHandle_NotificationRequest_WithValidGeofence()
    {
        // Arrange
        var serializer = new NotificationSerializer();
        var originalRequest = new NotificationRequest
        {
            NotificationId = 2,
            Title = "Geofence Notification",
            Description = "This is a geofence notification",
            Geofence = new NotificationRequestGeofence
            {
                Center = new NotificationRequestGeofence.Position
                {
                    Latitude = 37.7749,
                    Longitude = -122.4194
                },
                RadiusInMeters = 100
            }
        };

        // Act
        var json = serializer.Serialize(originalRequest);
        var deserializedRequest = serializer.Deserialize<NotificationRequest>(json);

        // Assert
        deserializedRequest.Should().NotBeNull();
        deserializedRequest.NotificationId.Should().Be(2);
        deserializedRequest.Geofence.Should().NotBeNull();
        deserializedRequest.Geofence.Center.Should().NotBeNull();
        deserializedRequest.Geofence.Center.Latitude.Should().Be(37.7749);
        deserializedRequest.Geofence.Center.Longitude.Should().Be(-122.4194);
        deserializedRequest.Geofence.RadiusInMeters.Should().Be(100);
        deserializedRequest.Geofence.IsGeofence.Should().BeTrue();
    }

    /// <summary>
    /// Tests that a Position object with NaN values can be serialized and deserialized correctly.
    /// </summary>
    [Fact]
    public void Serialize_Deserialize_ShouldHandle_Position_WithNaN()
    {
        // Arrange
        var serializer = new NotificationSerializer();
        var originalPosition = new NotificationRequestGeofence.Position
        {
            Latitude = double.NaN,
            Longitude = double.NaN
        };
        
        var geofence = new NotificationRequestGeofence
        {
            Center = originalPosition
        };

        var originalRequest = new NotificationRequest
        {
            NotificationId = 3,
            Title = "Test",
            Geofence = geofence
        };

        // Act
        var json = serializer.Serialize(originalRequest);
        var deserializedRequest = serializer.Deserialize<NotificationRequest>(json);

        // Assert
        deserializedRequest.Should().NotBeNull();
        deserializedRequest.Geofence.Center.Latitude.Should().Be(double.NaN);
        deserializedRequest.Geofence.Center.Longitude.Should().Be(double.NaN);
        deserializedRequest.Geofence.IsGeofence.Should().BeFalse();
    }

    /// <summary>
    /// Tests that a Position object with partial coordinates (one NaN, one valid) can be serialized and deserialized correctly.
    /// </summary>
    [Fact]
    public void Serialize_Deserialize_ShouldHandle_Position_WithPartialCoordinates()
    {
        // Arrange
        var serializer = new NotificationSerializer();
        var originalPosition = new NotificationRequestGeofence.Position
        {
            Latitude = 37.7749,
            Longitude = double.NaN
        };

        var geofence = new NotificationRequestGeofence
        {
            Center = originalPosition
        };

        var originalRequest = new NotificationRequest
        {
            NotificationId = 4,
            Title = "Test",
            Geofence = geofence
        };

        // Act
        var json = serializer.Serialize(originalRequest);
        var deserializedRequest = serializer.Deserialize<NotificationRequest>(json);

        // Assert
        deserializedRequest.Should().NotBeNull();
        deserializedRequest.Geofence.Center.Latitude.Should().Be(37.7749);
        deserializedRequest.Geofence.Center.Longitude.Should().Be(double.NaN);
        deserializedRequest.Geofence.IsGeofence.Should().BeFalse(); // Should be false because one coordinate is NaN
    }    
}
