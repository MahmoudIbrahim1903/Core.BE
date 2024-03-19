using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Emeint.Core.BE.LiveUpdates
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Run();
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
                        //.UseApplicationInsights()
                        .UseStartup<Startup>()
                        .Build();
    }
}
