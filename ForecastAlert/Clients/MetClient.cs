using System.Text.Json;
using Forecast.domain;
using Microsoft.Extensions.Logging;

namespace Forecast;

public class MetClient(ILogger<MetClient> logger, HttpClient httpClient, MetClientConfig config) : IMetClient
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task<LocationForecast?> GetLocationForecast(string latitude, string longitude)
    {
        var uri = $"https://api.met.no/weatherapi/locationforecast/2.0/compact?lat={latitude}&lon={longitude}";
        using var requestMessage = new HttpRequestMessage(
            HttpMethod.Get, uri);
        requestMessage.Headers.Add("User-Agent", config.UserAgent);
        var response = await httpClient.SendAsync(requestMessage);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<LocationForecast>(responseBody, _jsonSerializerOptions);
    }
}