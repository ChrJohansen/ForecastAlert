using System.Xml.Serialization;
using ForecastAlert.DTO;
using Microsoft.Extensions.Logging;

namespace ForecastAlert.Clients;

public class KartverketClient(HttpClient httpClient) : IKartverketClient
{
    private readonly XmlSerializer _xmlSerializer = new(typeof(Tide));

    public async Task<Tide?> GetTidalForecast(string latitude, string longitude)
    {
        var fromTime = DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm");
        var toTime = DateTimeOffset.Now.AddDays(3).ToString("yyyy-MM-ddTHH:mm");
        var uri =
            $"https://vannstand.kartverket.no/tideapi.php?lat={latitude}&lon={longitude}&fromtime={fromTime}&totime={toTime}&datatype=all&refcode=cd&place=&file=&lang=nn&interval=10&dst=1&tzone=&tide_request=locationdata";
        var response = await httpClient.GetAsync(
            uri);

        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();

        using var reader = new StringReader(responseBody);
        var forecast = (Tide)_xmlSerializer.Deserialize(reader)!;
        return forecast;
    }
}