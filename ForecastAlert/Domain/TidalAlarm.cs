using ForecastAlert.Domain;

namespace ForecastAlert.domain;

public class TidalAlarm : Alarm
{
    public int? MaxWaterLevels { get; set; }
}