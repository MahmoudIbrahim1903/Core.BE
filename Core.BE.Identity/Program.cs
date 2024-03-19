using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Emeint.Core.BE.Configurations.Infrastructure;
using Emeint.Core.BE.Identity.Infrastructure.Data;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Emeint.Core.BE.Identity
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args)
                .MigrateDbContext<ApplicationDbContext>((context, services) =>
                {
                    context.Seed();
                })
                .MigrateDbContext<CountryContext>((_, __) => { })
                .MigrateDbContext<ConfigurationDbContext>((context, services) =>
                {
                    var configuration = services.GetService<IConfiguration>();
                })
                .MigrateDbContext<ConfigurationContext>((context, services) =>
                {
                    var env = services.GetService<IHostingEnvironment>();
                    var settings = services.GetService<IOptions<AppSettings>>();
                }).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
             WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIIS()
                .UseSetting("detailedErrors", "true")
                .CaptureStartupErrors(true)
                .UseStartup<Startup>()
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                    config.AddEnvironmentVariables();
                })
                .ConfigureLogging((hostingContext, builder) =>
                {
                    builder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));

                    var logLevel = hostingContext.Configuration["Logging:LogLevel:Default"];
                    var logLevelParsedSucessfully = Enum.TryParse(typeof(LogLevel), logLevel, out object loglevelValue);

                    builder.AddConsole();
                    builder.AddDebug();
                    builder.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", logLevelParsedSucessfully ? (LogLevel)loglevelValue : LogLevel.Warning);
                })
                .UseApplicationInsights()
                .Build();
    }
}
