using Forecast.domain;

namespace Forecast;

public interface IMetClient
{
    public Task<LocationForecast?> GetLocationForecast(string  latitude, string longitude);
}