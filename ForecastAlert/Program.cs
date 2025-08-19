using System.Text.Json;
using Azure.Identity;
using Azure.Storage.Blobs;
using Forecast;
using ForecastAlert;
using ForecastAlert.Clients;
using ForecastAlert.domain;
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

    var credentials = new DefaultAzureCredential();
    var blobString = Environment.GetEnvironmentVariable("BLOB_URI");
    if (string.IsNullOrEmpty(blobString)) throw new Exception("BLOB_URI not set");

    var blobService = new BlobServiceClient(new Uri(blobString), credentials);
    var containerClient =  blobService.GetBlobContainerClient("forecast");
    var blobClient = containerClient.GetBlobClient("forecast");
    
    if (!await containerClient.ExistsAsync()) throw new Exception("Forecast alert not found");
    
    await using var stream = await blobClient.OpenReadAsync();
    var alarmConfig = await JsonSerializer.DeserializeAsync<AlarmConfig>(stream);
    if (alarmConfig == null) throw new Exception("No alarm config found");
    services.AddSingleton(alarmConfig);
    
});

var host = builder.Build();
host.Run();