using Forecast.domain;
using ForecastAlert.Domain;

namespace ForecastAlert.domain;

public class WindAlarm : Alarm
{
    public int MaxWindSpeed { get; set; }
    public List<WindDirection> Directions { get; set; } = [];
}