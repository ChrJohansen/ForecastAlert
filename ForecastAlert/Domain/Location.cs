namespace ForecastAlert.Domain;

public class Location
{
    public required string Latitude { get; set; }
    public required string Longitude { get; set; }
    public required string LocationName { get; set; }

    public List<Alarm> Alarms { get; set; } = [];
}