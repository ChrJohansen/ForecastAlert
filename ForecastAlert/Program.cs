using ForecastAlert.Clients;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureFunctionsWorkerDefaults();
builder.ConfigureServices(services => { services.AddHttpClient<IKartverketClient, KartverketClient>(); });

var host = builder.Build();
host.Run();