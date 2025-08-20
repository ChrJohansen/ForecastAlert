using ForecastAlert.Domain;

namespace ForecastAlert.Services;

public interface ILocationService
{
    public Task HandleLocation(Location location);
}