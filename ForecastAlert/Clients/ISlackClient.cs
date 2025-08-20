namespace ForecastAlert.Clients;

public interface ISlackClient
{
    public void publish(string message);
}