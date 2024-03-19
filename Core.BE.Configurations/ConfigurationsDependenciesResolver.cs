using Emeint.Core.BE.Configurations.Domain.Model;
using Emeint.Core.BE.Configurations.Infrastructure;
using Emeint.Core.BE.Configurations.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Configurations
{
    public static class ConfigurationsDependenciesResolver
    {
        public static void Register(IServiceCollection services, IConfiguration configuration)
        {
            if (configuration["DataBaseType"] == "Cosmos")
                services.AddScoped<ISettingRepository, SettingCosmosRepository>();
            else
                services.AddScoped<ISettingRepository, SettingRepository>();
        }
    }
}
