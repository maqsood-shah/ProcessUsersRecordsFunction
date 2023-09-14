using HandleNotification.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        // Add configuration sources as needed
        config.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddJsonFile("local.host.json", optional: true, reloadOnChange: true) // Add this line to use local.host.json
            .AddEnvironmentVariables()
            .AddCommandLine(args);
    })
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;
        services.AddSingleton<IAzureCommunicationServiceRepository>(provider =>
        {
            return new AzureCommunicationServiceRepository(hostContext.Configuration);
        });
        services.AddLogging();
    })
    .ConfigureFunctionsWorkerDefaults()
    .Build();

host.Run();
