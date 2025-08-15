using ForecastAlert.Clients;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ForecastAlert.AzureFunctions;

public class ForecastChecker(ILogger<ForecastChecker> logger, IKartverketClient kartverketClient)
{
    [Function(nameof(ForecastChecker))]
    [FixedDelayRetry(5, "00:00:05")]
    public async Task Run([TimerTrigger("0 * * * * *", RunOnStartup = true)] TimerInfo timerInfo)
    {
        var tide = await kartverketClient.GetTidalForecast(latitude: "59.91273", longitude: "10.74609");
        logger.LogInformation("Got tidal info!");
    }
}