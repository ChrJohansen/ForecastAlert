using ForecastAlert.Clients;
using ForecastAlert.Domain;
using Microsoft.Extensions.Logging;

namespace ForecastAlert.Services;

public class AlarmService(
    ILogger<AlarmService> logger,
    IKartverketClient kartverketClient,
    ISlackClient slackClient,
    IMetClient metClient)
    : IAlarmService
{
    private async Task HandleTidalAlarm(TidalAlarm tidalAlarm, string latitude, string longitude, string locationName)
    {
        if (tidalAlarm.MaxWaterLevels == null)
        {
            logger.LogInformation("TidalAlarm max is undefined");
            return;
        }

        logger.LogInformation("Getting Tidal Forecast for: {LocationName}", locationName);
        var tide = await kartverketClient.GetTidalForecast(latitude, longitude);

        if (tide == null)
        {
            logger.LogInformation("Tidal Forecast for: {LocationName} not found. Try again later", locationName);
            return;
        }

        var forecasts = tide!.LocationData.Data.Where(d => d.Type == "forecast").ToList();
        var measurements = forecasts.SelectMany(d => d.WaterLevels).Where(wl => wl.Value > tidalAlarm.MaxWaterLevels)
            .ToList();

        if (measurements.Count != 0)
        {
            var times = measurements.Select(i => DateTimeOffset.Parse(i.Time).ToString("yy-MM-dd")).Distinct().ToList();
            var message =
                $"Det er meldt høy vannstand (Over {tidalAlarm.MaxWaterLevels}cm) ved {locationName} på følgende dager:\n{string.Join("\n", times)}";
            slackClient.publish(message);
        }
        logger.LogInformation("No high water levels for {LocationName} in the next few days", locationName);
    }

    private WindDirection DegreesToWindDirection(double degrees)
    {
        var enumIndex = (int)Math.Floor((degrees + 11.25) / 22.5);
        return (WindDirection)(enumIndex % 16);
    }

    private async Task HandleWindAlarm(WindAlarm windAlarm, string latitude, string longitude, string locationName)
    {
        logger.LogInformation("Getting Wind Forecast for: {LocationName}", locationName);
        var forecast = await metClient.GetLocationForecast(latitude, longitude);

        if (forecast == null)
        {
            logger.LogInformation("Wind Forecast for: {LocationName} not found", locationName);
            return;
        }

        var timeSeries =
            forecast.Properties.TimeSeries.Where(ts => ts.Data.Instant.Details.WindSpeed > windAlarm.MaxWindSpeed)
                .ToList();

        if (timeSeries.Count == 0)
        {
            logger.LogInformation("No wind speed alarm from {LocationName} in the next few days", locationName);
            return;
        }

        List<string> lines;

        if (windAlarm.Directions.Count != 0)
        {
            timeSeries = timeSeries.Where(m =>
                    windAlarm.Directions.Contains(
                        DegreesToWindDirection(m.Data.Instant.Details.WindFromDirection!.Value)))
                .ToList();

            if (timeSeries.Count == 0)
                return;

            lines = timeSeries.Select(ts =>
                ts.Time.ToString("yy-MM-dd") + "(" +
                DegreesToWindDirection(ts.Data.Instant.Details.WindFromDirection!.Value) + " )").Distinct().ToList();
        }
        else
        {
            lines = timeSeries
                .Select(ts => ts.Time.ToString("yy-MM-dd"))
                .Distinct()
                .ToList();
        }

        var message =
            $"Det er meldt kraftig vind (Over {windAlarm.MaxWindSpeed}m/s) ved {locationName} på følgende dager:\n{string.Join("\n", lines)}";
        slackClient.publish(message);
    }

    public async Task HandleAlarms(Location location)
    {
        foreach (var alarm in location.Alarms)
        {
            switch (alarm)
            {
                case TidalAlarm tidalAlarm:
                    await HandleTidalAlarm(tidalAlarm, location.Latitude, location.Longitude,
                        location.LocationName);
                    break;
                case WindAlarm windAlarm:
                    await HandleWindAlarm(windAlarm, location.Latitude, location.Longitude,
                        location.LocationName);
                    break;
                default:
                    logger.LogError("Invalid alarm type");
                    break;
            }
        }
    }
}