using Emeint.Core.BE.Configurations.Infrastructure;
using Emeint.Core.BE.Notifications.Infrastructure;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.BuildingBlocks.IntegrationEventLogEF;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace Emeint.Core.BE.Notifications
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args)
                .MigrateDbContext<MessagingContext>((context, services) =>
                {
                    var env = services.GetService<IHostingEnvironment>();
                })
                .MigrateDbContext<ConfigurationContext>((context, services) =>
                {
                    var env = services.GetService<IHostingEnvironment>();
                })
                .MigrateDbContext<IntegrationEventLogContext>((_, __) => { })
                .Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                // if you want to control Kestrel Port
                //.UseConfiguration(configuration)
                //.UseHealthChecks("/hc")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureKestrel(options => { options.AllowSynchronousIO = true; })
                .UseIIS()
                .UseSetting("detailedErrors", "true")
                .CaptureStartupErrors(true)
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                    config.AddJsonFile("appsettings.json");
                    config.AddEnvironmentVariables();
                })
                .ConfigureLogging((hostingContext, builder) =>
                {
                    builder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    var logLevel = hostingContext.Configuration["Logging:LogLevel:DatabaseCommand"];
                    var logLevelParsedSucessfully = Enum.TryParse(typeof(LogLevel), logLevel, out object loglevelValue);

                    builder.AddConsole();
                    builder.AddDebug();
                    builder.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", logLevelParsedSucessfully ? (LogLevel)loglevelValue : LogLevel.Warning);
                })
                .UseApplicationInsights()
                .Build();
    }
}
