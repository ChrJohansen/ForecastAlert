using ForecastAlert.Domain;
using Microsoft.Extensions.Logging;

namespace ForecastAlert.Services;

public class LocationService(ILogger<LocationService> logger, IAlarmService alarmService): ILocationService
{
    public async Task HandleLocation(Location location)
    {
        logger.LogInformation("Handling alarms for location: {LocationName}", location.LocationName);
        if (location.Alarms.Count == 0)
        {
            logger.LogInformation("No alarms to handle");
            return;
        }
        
        await alarmService.HandleAlarms(location);
    }
}