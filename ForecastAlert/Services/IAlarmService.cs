using ForecastAlert.Domain;

namespace ForecastAlert.Services;

public interface IAlarmService
{
    public Task HandleAlarms(Location location);
}