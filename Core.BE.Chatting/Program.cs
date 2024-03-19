using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Emeint.Core.BE.Chatting.Infractructure.Data;
using Emeint.Core.BE.Configurations.Infrastructure;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Emeint.Core.BE.Chatting
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args)
                .MigrateDbContext<ChattingDbContext>((context, services) => { })
                .MigrateDbContext<ConfigurationContext>((context, services) => { })
                .Run();
        }
        public static IWebHost CreateWebHostBuilder(string[] args) =>
                    WebHost.CreateDefaultBuilder(args).UseKestrel()
                        //.UseHealthChecks("/hc")
                        .UseContentRoot(Directory.GetCurrentDirectory())
                        .UseIIS()
                        .UseSetting("detailedErrors", "true")
                        .CaptureStartupErrors(true)
                        .UseStartup<Startup>()
                        .ConfigureAppConfiguration((builderContext, config) =>
                        {
                            config.AddEnvironmentVariables();
                        })
                        .UseApplicationInsights()
                        .ConfigureLogging((hostingContext, builder) =>
                        {
                            builder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));

                            var logLevel = hostingContext.Configuration["Logging:LogLevel:DatabaseCommand"];
                            var logLevelParsedSucessfully = Enum.TryParse(typeof(LogLevel), logLevel, out object loglevelValue);

                            builder.AddConsole();
                            builder.AddDebug();
                            builder.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", logLevelParsedSucessfully ? (LogLevel)loglevelValue : LogLevel.Warning);
                        })
                        .UseStartup<Startup>()
                        .Build();
    }
}
