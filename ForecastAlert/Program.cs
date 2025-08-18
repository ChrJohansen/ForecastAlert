using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
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
    
    var keyVaultName = Environment.GetEnvironmentVariable("KEY_VAULT_NAME");
    var keyVaultUrl = $"https://{keyVaultName}.vault.azure.net/";
    var client = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());

    const string secretName = "SlackKey";
    var slackKey = await client.GetSecretAsync(secretName);
    
    services.AddSingleton(new SlackConfig { SlackKey = slackKey.Value.Value });
    
    
    services.AddSingleton<IAlarmService, AlarmService>();
    services.AddSingleton<ILocationService, LocationService>();
});

var host = builder.Build();
host.Run();