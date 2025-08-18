using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace ForecastAlert.AzureFunctions;

public class KeepAlive(ILogger<ForecastChecker> logger)
{
    [Function(nameof(KeepAlive))]
    public HttpResponseData Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
        HttpRequestData request)
    {
        logger.LogInformation("Keep alive ping received. App is warm");
        var response = request.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
        response.WriteString("Keep/alive pinged. App is warm");

        return response;
    }
}