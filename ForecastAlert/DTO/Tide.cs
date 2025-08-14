using System.Xml.Serialization;

namespace ForecastAlert.DTO;

[XmlRoot("tide")]
public class Tide
{
    [XmlElement("locationdata")] public LocationData LocationData { get; set; }
}

public class LocationData
{
    [XmlElement("location")] public Location Location { get; set; }

    [XmlElement("reflevelcode")] public string RefLevelCode { get; set; }

    [XmlElement("data")] public List<Data> Data { get; set; }
}

public class Location
{
    [XmlAttribute("name")] public string Name { get; set; }

    [XmlAttribute("code")] public string Code { get; set; }

    [XmlAttribute("latitude")] public double Latitude { get; set; }

    [XmlAttribute("longitude")] public double Longitude { get; set; }

    [XmlAttribute("delay")] public int Delay { get; set; }

    [XmlAttribute("factor")] public double Factor { get; set; }

    [XmlAttribute("obsname")] public string ObsName { get; set; }

    [XmlAttribute("obscode")] public string ObsCode { get; set; }

    [XmlAttribute("descr")] public string Description { get; set; }
}

public class Data
{
    [XmlAttribute("type")] public string Type { get; set; }

    [XmlAttribute("unit")] public string Unit { get; set; }

    [XmlAttribute("qualityFlag")] public int QualityFlag { get; set; }

    [XmlAttribute("qualityClass")] public string QualityClass { get; set; }

    [XmlAttribute("qualityDescription")] public string QualityDescription { get; set; }

    [XmlElement("waterlevel")] public List<WaterLevel> WaterLevels { get; set; }
}

public class WaterLevel
{
    [XmlAttribute("value")] public double Value { get; set; }

    [XmlAttribute("time")] public string Time { get; set; }

    [XmlAttribute("flag")] public string Flag { get; set; }
}