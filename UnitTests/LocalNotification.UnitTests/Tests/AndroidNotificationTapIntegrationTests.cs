namespace Plugin.LocalNotification.UnitTests.Tests;

/// <summary>
/// Integration tests for the Android notification tap scenario with geofence NaN values.
/// This test simulates the real-world scenario where:
/// 1. A notification is triggered without geofence
/// 2. The notification is serialized to JSON 
/// 3. The JSON is stored in Intent.Extras
/// 4. When the notification is tapped, the JSON is retrieved and deserialized
/// </summary>
public class AndroidNotificationTapIntegrationTests
{
    /// <summary>
    /// Simulates the Android scenario: trigger notification without geofence, 
    /// serialize it, and then deserialize it when the notification is tapped.
    /// </summary>
    [Fact]
    public void Android_NotificationTap_ShouldDeserializeCorrectly_WithoutGeofence()
    {
        // Arrange
        var serializer = new NotificationSerializer();
        
        // Step 1: Create a notification request without geofence (as would be done in ShowNotification)
        var notificationRequest = new NotificationRequest
        {
            NotificationId = 42,
            Title = "Sample Notification",
            Description = "This is a sample notification without geofence",
            BadgeNumber = 1,
            ReturningData = "custom_data_123"
        };

        // Step 2: Serialize the request (as would be done in GetRequestSerialize)
        var jsonString = serializer.Serialize(notificationRequest);
        
        // The JSON should contain geofence with NaN values
        jsonString.Should().Contain("geofence");
        jsonString.Should().NotContain("\"latitude\":null");  // Should not be null
        jsonString.Should().NotContain("\"longitude\":null"); // Should not be null

        // Step 3: This simulates storing in Intent.Extras.PutExtra("Plugin.LocalNotification.RETURN_REQUEST", jsonString)
        var storedJson = jsonString;

        // Step 4: This simulates retrieving from Intent.Extras.GetString("Plugin.LocalNotification.RETURN_REQUEST")
        var retrievedJson = storedJson;

        // Step 5: Deserialize when notification is tapped (this is where the bug occurred)
        var deserializedRequest = serializer.Deserialize<NotificationRequest>(retrievedJson);

        // Assert
        deserializedRequest.Should().NotBeNull("Deserialization should succeed");
        deserializedRequest.NotificationId.Should().Be(42);
        deserializedRequest.Title.Should().Be("Sample Notification");
        deserializedRequest.Description.Should().Be("This is a sample notification without geofence");
        deserializedRequest.BadgeNumber.Should().Be(1);
        deserializedRequest.ReturningData.Should().Be("custom_data_123");
        deserializedRequest.Geofence.Should().NotBeNull("Geofence should be deserialized");
        deserializedRequest.Geofence.Center.Should().NotBeNull("Geofence Center should be deserialized");
        deserializedRequest.Geofence.Center.Latitude.Should().Be(double.NaN, "Latitude should be NaN");
        deserializedRequest.Geofence.Center.Longitude.Should().Be(double.NaN, "Longitude should be NaN");
        deserializedRequest.Geofence.IsGeofence.Should().BeFalse("IsGeofence should be false when coordinates are NaN");
    }

    /// <summary>
    /// Simulates the Android scenario with valid geofence coordinates.
    /// </summary>
    [Fact]
    public void Android_NotificationTap_ShouldDeserializeCorrectly_WithGeofence()
    {
        // Arrange
        var serializer = new NotificationSerializer();
        
        // Create a notification request WITH geofence
        var notificationRequest = new NotificationRequest
        {
            NotificationId = 43,
            Title = "Geofence Notification",
            Description = "This is a geofence-based notification",
            Geofence = new NotificationRequestGeofence
            {
                Center = new NotificationRequestGeofence.Position
                {
                    Latitude = 40.7128,
                    Longitude = -74.0060
                },
                RadiusInMeters = 250
            }
        };

        // Serialize
        var jsonString = serializer.Serialize(notificationRequest);
        
        // Deserialize
        var deserializedRequest = serializer.Deserialize<NotificationRequest>(jsonString);

        // Assert
        deserializedRequest.Should().NotBeNull();
        deserializedRequest.NotificationId.Should().Be(43);
        deserializedRequest.Geofence.Center.Latitude.Should().Be(40.7128);
        deserializedRequest.Geofence.Center.Longitude.Should().Be(-74.0060);
        deserializedRequest.Geofence.RadiusInMeters.Should().Be(250);
        deserializedRequest.Geofence.IsGeofence.Should().BeTrue();
    }

    /// <summary>
    /// Tests multiple notifications serialized and deserialized in sequence.
    /// This simulates handling multiple notification taps.
    /// </summary>
    [Fact]
    public void Android_MultipleNotifications_ShouldSerializeDeserializeCorrectly()
    {
        // Arrange
        var serializer = new NotificationSerializer();
        var requests = new List<NotificationRequest>
        {
            new NotificationRequest { NotificationId = 1, Title = "First", Description = "No geofence" },
            new NotificationRequest 
            { 
                NotificationId = 2, 
                Title = "Second with geofence",
                Geofence = new NotificationRequestGeofence
                {
                    Center = new NotificationRequestGeofence.Position
                    {
                        Latitude = 51.5074,
                        Longitude = -0.1278
                    }
                }
            },
            new NotificationRequest { NotificationId = 3, Title = "Third", Description = "No geofence" }
        };

        // Act
        var jsonList = serializer.Serialize(requests);
        var deserializedList = serializer.Deserialize<List<NotificationRequest>>(jsonList);

        // Assert
        deserializedList.Should().HaveCount(3);
        
        // First notification (no geofence)
        deserializedList[0].NotificationId.Should().Be(1);
        deserializedList[0].Geofence.IsGeofence.Should().BeFalse();
        
        // Second notification (with geofence)
        deserializedList[1].NotificationId.Should().Be(2);
        deserializedList[1].Geofence.IsGeofence.Should().BeTrue();
        deserializedList[1].Geofence.Center.Latitude.Should().Be(51.5074);
        
        // Third notification (no geofence)
        deserializedList[2].NotificationId.Should().Be(3);
        deserializedList[2].Geofence.IsGeofence.Should().BeFalse();
    }
}
