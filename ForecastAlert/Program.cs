using Forecast;
using ForecastAlert;
using ForecastAlert.Clients;
using ForecastAlert.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureFunctionsWorkerDefaults();
builder.ConfigureServices(async void (services) =>
{
    services.AddHttpClient<IKartverketClient, KartverketClient>();
    services.AddHttpClient<IMetClient, MetClient>();
    services.AddHttpClient<ISlackClient, SlackClient>();
    
    var slackKey = Environment.GetEnvironmentVariable("SLACK_KEY");
    
    if (string.IsNullOrEmpty(slackKey)) throw new Exception("SLACK_KEY not set");
    
    services.AddSingleton(new SlackConfig { SlackKey = slackKey });
    
    services.AddSingleton<IAlarmService, AlarmService>();
    services.AddSingleton<ILocationService, LocationService>();
});

var host = builder.Build();
host.Run();