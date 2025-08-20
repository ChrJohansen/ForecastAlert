using System.Text.Json.Serialization;

namespace ForecastAlert.Domain;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "AlarmClass")]
[JsonDerivedType(typeof(TidalAlarm), typeDiscriminator: "TidalClass")]
[JsonDerivedType(typeof(WindAlarm), typeDiscriminator: "WindClass")]
public abstract class Alarm
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AlarmType AlarmType { get; set; }
    public required string Name { get; set; }
}