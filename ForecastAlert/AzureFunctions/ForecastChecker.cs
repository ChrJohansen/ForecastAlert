using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ForecastAlert.AzureFunctions;

public class ForecastChecker(ILogger<ForecastChecker> logger)
{
    [Function(nameof(ForecastChecker))]
    [FixedDelayRetry(5, "00:00:05")]
    public void Run([TimerTrigger("* * /1 * * *")] TimerInfo timerInfo)
    {
        // TODO
    }
}