using ForecastAlert.DTO;

namespace ForecastAlert.Clients;

public interface IKartverketClient
{
    public Task<Tide?> GetTidalForecast(string latitude, string longitude, int daysToLookAhead);
}