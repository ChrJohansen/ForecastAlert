using Forecast;
using ForecastAlert;
using ForecastAlert.Clients;
using ForecastAlert.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureFunctionsWorkerDefaults();
builder.ConfigureServices(services =>
{
    services.AddHttpClient<IKartverketClient, KartverketClient>();
    services.AddHttpClient<IMetClient, MetClient>();
    services.AddHttpClient<ISlackClient, SlackClient>();
    
    services.AddSingleton(new SlackConfig { SlackKey = "TODO" });
    services.AddSingleton<IAlarmService, AlarmService>();
    services.AddSingleton<ILocationService, LocationService>();
});

var host = builder.Build();
host.Run();