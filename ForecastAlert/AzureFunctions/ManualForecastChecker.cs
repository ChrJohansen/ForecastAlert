using System.Net;
using ForecastAlert.Domain;
using ForecastAlert.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace ForecastAlert.AzureFunctions;

public class ManualForecastChecker(
    ILogger<ForecastChecker> logger,
    AlarmConfig alarmConfig,
    ILocationService locationService)
{
    [Function(nameof(ManualForecastChecker))]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "/manual")]
        HttpRequestData req)
    {
        logger.LogInformation("ForecastChecker has been manually triggered");
        foreach (var location in alarmConfig.Locations)
        {
            await locationService.HandleLocation(location);
        }

        var response = req.CreateResponse(HttpStatusCode.OK);
        return response;
    }
}