using log4net.Config;
using log4net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using SendNotificaton.Repositories;

var host = new HostBuilder()
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        // Add configuration sources as needed
        config.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddJsonFile("local.host.json", optional: true) // Add this line to use local.host.json
            .AddEnvironmentVariables()
            .AddCommandLine(args);
    })
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;
        // Add services here
        services.AddDbContext<UsersRecordsContext>(options =>
        {
            options.UseSqlServer(configuration["SqlServerConnectionString"]);
        });
        services.AddSingleton<IServiceBusRepository>(provider =>
        {
            var serviceBusConnectionString = configuration["AzureWebJobsServiceBus"];
            var serviceBusQueueName = configuration["ServiceBusQueueName"];
            return new ServiceBusRepository(serviceBusConnectionString, serviceBusQueueName);
        });
        services.AddTransient<IUserRecordsContextRepository, UserRecordsContextRepository>();
        // Configure log4net
        var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
        XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
    })
    .ConfigureFunctionsWorkerDefaults()
    .Build();

host.Run();
