using ForecastAlert.Domain;
using ForecastAlert.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace ForecastAlert.Tests.Services;

public class LocationServiceTests
{
    private readonly Mock<ILogger<LocationService>> _mockLogger;
    private readonly Mock<IAlarmService> _mockAlarmService;
    private readonly LocationService _locationService;

    public LocationServiceTests()
    {
        _mockLogger = new Mock<ILogger<LocationService>>();
        _mockAlarmService = new Mock<IAlarmService>();
        _locationService = new LocationService(_mockLogger.Object, _mockAlarmService.Object);
    }

    [Fact]
    public async Task should_log_if_no_alarms()
    {
        var location = new Location
        {
            Latitude = "123",
            Longitude = "456",
            LocationName = "Test",
            Alarms = []
        };

        await _locationService.HandleLocation(location);
        _mockLogger.Verify(
            x =>
                x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("No alarms to handle")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);

        _mockAlarmService.Verify(a => a.HandleAlarms(It.IsAny<Location>()), Times.Never);
    }

    [Fact]
    public async Task should_call_alarm_service_if_alarms()
    {
        var alarm = new WindAlarm
        {
            Name = "AName"
        };
        var location = new Location
        {
            Latitude = "123",
            Longitude = "456",
            LocationName = "Test",
            Alarms = [alarm]
        };

        await _locationService.HandleLocation(location);


        _mockAlarmService.Verify(a => a.HandleAlarms(location), Times.Once);
    }
}