using ForecastAlert.Domain;
using ForecastAlert.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ForecastAlert.AzureFunctions;

public class ForecastChecker(ILogger<ForecastChecker> logger, AlarmConfig alarmConfig, ILocationService locationService)
{
    [Function(nameof(ForecastChecker))]
    [FixedDelayRetry(5, "00:00:05")]
    public async Task Run([TimerTrigger("%ForecastSchedule%")] TimerInfo timerInfo)
    {
        foreach (var location in alarmConfig.Locations)
        {
            await locationService.HandleLocation(location);
        }
    }
}