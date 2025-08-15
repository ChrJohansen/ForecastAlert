namespace ForecastAlert;

public interface ISlackClient
{
    public void publish(string message);
}