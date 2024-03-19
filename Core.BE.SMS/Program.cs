using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Emeint.Core.BE.Configurations.Infrastructure;
using Emeint.Core.BE.SMS.Infractructure.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Emeint.Core.BE.SMS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args)
                .MigrateDbContext<SmsDbContext>((context, services) => { })
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
                .UseStartup<Startup>()
                .Build();

    }
}
