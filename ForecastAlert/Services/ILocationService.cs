using ForecastAlert.domain;

namespace ForecastAlert.Services;

public interface ILocationService
{
    public void HandleLocation(Location location);
}