
using ForecastAlert.Domain;
using ForecastAlert.DTO;

namespace ForecastAlert.Clients;

public interface IMetClient
{
    public Task<LocationForecast?> GetLocationForecast(string  latitude, string longitude);
}