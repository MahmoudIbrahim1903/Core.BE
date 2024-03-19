using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Emeint.Core.BE.Mailing.Infrastructure;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
//using Emeint.Core.BE.Media.Infrastructure;
using Microsoft.BuildingBlocks.IntegrationEventLogEF;
using Emeint.Core.BE.Configurations.Infrastructure;

namespace Emeint.Core.BE.Mailing
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args)
             .MigrateDbContext<MailingContext>((context, services) =>
            {
                var env = services.GetService<IHostingEnvironment>();
                var settings = services.GetService<IOptions<AppSettings>>();
            })
             .MigrateDbContext<ConfigurationContext>((context, services) =>
             {
                 var env = services.GetService<IHostingEnvironment>();
                 var settings = services.GetService<IOptions<AppSettings>>();
             })
             .MigrateDbContext<IntegrationEventLogContext>((_, __) => { })
                .Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseContentRoot(Directory.GetCurrentDirectory())
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
                    builder.AddConsole();
                    builder.AddDebug();
                    builder.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
                })
                .UseApplicationInsights()
                .Build();
    }
}
