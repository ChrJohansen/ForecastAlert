
namespace ForecastAlert.Domain;

public class TidalAlarm : Alarm
{
    public int? MaxWaterLevels { get; set; }
    public int? DaysToLookAhead { get; set; }
}