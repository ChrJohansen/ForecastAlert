using System.Text.Json;
using System.Xml.Serialization;
using ForecastAlert.Clients;
using ForecastAlert.Domain;
using ForecastAlert.DTO;
using ForecastAlert.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Location = ForecastAlert.Domain.Location;

namespace ForecastAlert.Tests.Services;

public class AlarmServiceTests
{
    private readonly Mock<ILogger<AlarmService>> _loggerMock;
    private readonly Mock<IKartverketClient> _kartverketClientMock;
    private readonly Mock<ISlackClient> _slackClientMock;
    private readonly Mock<IMetClient> _metClientMock;
    private readonly AlarmService _alarmService;

    public AlarmServiceTests()
    {
        _loggerMock = new Mock<ILogger<AlarmService>>();
        _kartverketClientMock = new Mock<IKartverketClient>();
        _slackClientMock = new Mock<ISlackClient>();
        _metClientMock = new Mock<IMetClient>();
        _alarmService = new AlarmService(_loggerMock.Object, _kartverketClientMock.Object, _slackClientMock.Object,
            _metClientMock.Object);
    }

    [Fact]
    public async Task wind_alarm_should_handle_no_forecast_found()
    {
        var alarm = new WindAlarm()
        {
            Name = "AName",
            MaxWindSpeed = 22
        };
        var location = new Location
        {
            Latitude = "123",
            Longitude = "456",
            LocationName = "Test",
            Alarms = [alarm]
        };

        await _alarmService.HandleAlarms(location);
        _metClientMock.Verify(m => m.GetLocationForecast(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

        _loggerMock.Verify(
            x =>
                x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("not found")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }

    [Fact]
    public async Task wind_alarm_should_handle_forecast_found()
    {
        var alarm = new WindAlarm()
        {
            Name = "AName",
            MaxWindSpeed = 1
        };
        var location = new Location
        {
            Latitude = "123",
            Longitude = "456",
            LocationName = "Test",
            Alarms = [alarm]
        };

        JsonSerializerOptions jsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var filePath = Path.Combine(AppContext.BaseDirectory, "data", "forecast.json");
        var jsonString = await File.ReadAllTextAsync(filePath);

        var output = JsonSerializer.Deserialize<LocationForecast>(jsonString, jsonSerializerOptions);

        _metClientMock.Setup(x => x.GetLocationForecast(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(output);


        await _alarmService.HandleAlarms(location);
        _metClientMock.Verify(m => m.GetLocationForecast(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

        _loggerMock.Verify(
            x =>
                x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("not found")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Never);
        _slackClientMock.Verify(m => m.publish(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task tidal_alarm_should_handle_no_tide_found()
    {
        var alarm = new TidalAlarm
        {
            Name = "AName",
            MaxWaterLevels = 122
        };
        var location = new Location
        {
            Latitude = "123",
            Longitude = "456",
            LocationName = "Test",
            Alarms = [alarm]
        };

        await _alarmService.HandleAlarms(location);
        _kartverketClientMock.Verify(c => c.GetTidalForecast(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
            Times.Once);

        _loggerMock.Verify(
            x =>
                x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("not found")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }

    [Fact]
    public async Task tidal_alarm_should_notify_slack()
    {
        var alarm = new TidalAlarm
        {
            Name = "AName",
            MaxWaterLevels = 97
        };
        var location = new Location
        {
            Latitude = "123",
            Longitude = "456",
            LocationName = "Test",
            Alarms = [alarm]
        };

        var filePath = Path.Combine(AppContext.BaseDirectory, "data", "tide.xml");
        var xmlString = await File.ReadAllTextAsync(filePath);
        XmlSerializer xmlSerializer = new(typeof(Tide));
        using var reader = new StringReader(xmlString);
        var tide = (Tide)xmlSerializer.Deserialize(reader)!;


        _kartverketClientMock.Setup(x => x.GetTidalForecast(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(
                tide);

        await _alarmService.HandleAlarms(location);
        _kartverketClientMock.Verify(c => c.GetTidalForecast(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()));


        _loggerMock.Verify(
            x =>
                x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("not found")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Never);

        _slackClientMock.Verify(m => m.publish(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task tidal_alarm_should_not_notify_slack()
    {
        var alarm = new TidalAlarm
        {
            Name = "AName",
            MaxWaterLevels = 500
        };
        var location = new Location
        {
            Latitude = "123",
            Longitude = "456",
            LocationName = "Test",
            Alarms = [alarm]
        };

        var filePath = Path.Combine(AppContext.BaseDirectory, "data", "tide.xml");
        var xmlString = await File.ReadAllTextAsync(filePath);
        XmlSerializer xmlSerializer = new(typeof(Tide));
        using var reader = new StringReader(xmlString);
        var tide = (Tide)xmlSerializer.Deserialize(reader)!;


        _kartverketClientMock.Setup(x => x.GetTidalForecast(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(
                tide);

        await _alarmService.HandleAlarms(location);
        _kartverketClientMock.Verify(c => c.GetTidalForecast(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()));


        _loggerMock.Verify(
            x =>
                x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("not found")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Never);

        _slackClientMock.Verify(m => m.publish(It.IsAny<string>()), Times.Never);
    }
}