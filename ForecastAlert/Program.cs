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
builder.ConfigureServices(void (services) =>
{
    services.AddHttpClient<IKartverketClient, KartverketClient>();
    services.AddHttpClient<IMetClient, MetClient>();
    services.AddHttpClient<ISlackClient, SlackClient>();

    var slackKey = Environment.GetEnvironmentVariable("SLACK_KEY");
    if (string.IsNullOrEmpty(slackKey)) throw new Exception("SLACK_KEY not set");
    services.AddSingleton(new SlackConfig { SlackKey = slackKey });
    
    
    var metUserAgent = Environment.GetEnvironmentVariable("MET_USER_AGENT");
    if (string.IsNullOrEmpty(metUserAgent)) throw new Exception("MET_USER_AGENT not set");
    services.AddSingleton(new MetClientConfig() { UserAgent = metUserAgent });
    
    
    services.AddSingleton<IAlarmService, AlarmService>();
    services.AddSingleton<ILocationService, LocationService>();

    var credentials = new DefaultAzureCredential();
    var blobString = Environment.GetEnvironmentVariable("BLOB_URI");
    if (string.IsNullOrEmpty(blobString)) throw new Exception("BLOB_URI not set");

    var blobService = new BlobServiceClient(new Uri(blobString), credentials);
    
    var blobContainerName =  Environment.GetEnvironmentVariable("BLOB_CONTAINER");
    if (string.IsNullOrEmpty(blobContainerName)) throw new Exception("BLOB_CONTAINER not set");
    var containerClient =  blobService.GetBlobContainerClient(blobContainerName);
    
    var blobName = Environment.GetEnvironmentVariable("BLOB_NAME");
    if (string.IsNullOrEmpty(blobName)) throw new Exception("BLOB_NAME not set");
    var blobClient = containerClient.GetBlobClient(blobName);
    
    if (! containerClient.Exists()) throw new Exception("Forecast alert not found");
    
    using var stream = blobClient.OpenRead();
    var alarmConfig = JsonSerializer.Deserialize<AlarmConfig>(stream);
    if (alarmConfig == null) throw new Exception("No alarm config found");
    services.AddSingleton(alarmConfig);
    
});

var host = builder.Build();
host.Run();