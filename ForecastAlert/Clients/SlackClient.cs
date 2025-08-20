using System.Text.Json;

namespace ForecastAlert.Clients;

public class SlackClient(HttpClient httpClient, SlackConfig slackConfig) : ISlackClient
{
    public void publish(string message)
    {
        var json = JsonSerializer.Serialize(new { my_message =  message }) ;

        var response = httpClient.PostAsync(
            slackConfig.SlackKey,
            new StringContent(json)).Result;

        response.EnsureSuccessStatusCode();
    }
}