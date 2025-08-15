using ForecastAlert.domain;

namespace ForecastAlert.Services;

public interface IAlarmService
{
    public Task HandleAlarms(Location location);
}