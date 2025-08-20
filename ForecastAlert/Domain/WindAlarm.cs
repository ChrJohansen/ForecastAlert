namespace ForecastAlert.Domain;

public class WindAlarm : Alarm
{
    public int MaxWindSpeed { get; set; }
    public List<WindDirection> Directions { get; set; } = [];
}