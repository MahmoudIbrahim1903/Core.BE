//using IdentityServer4.EntityFramework.DbContexts;
//using IdentityServer4.EntityFramework.Mappers;
//using Emeint.Core.BE.Identity.Configuration;
//using Microsoft.Extensions.Configuration;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Emeint.Core.BE.Identity.Infrastructure.Data
//{
//    public class IdentityServerConfigurationDbContextSeed
//    {
//        public async Task SeedAsync(ConfigurationDbContext context, IConfiguration configuration, IIdentityDataConfig identityDataConfig)
//        {

//            //callbacks urls from config:
//            var clientUrls = new Dictionary<string, string>();

//            clientUrls.Add("ResourceOwnerPassword", configuration.GetValue<string>("ResourceOwnerPassword"));
//            clientUrls.Add("Xamarin", "http://localhost:5105/xamarincallback"
//                // , configuration.GetValue<string>("XamarinCallback")
//                );
//            clientUrls.Add("IdentityApi", configuration.GetValue<string>("IdentityApiClient"));
//            clientUrls.Add("VehicleApi", configuration.GetValue<string>("VehicleApiClient"));
//            clientUrls.Add("MessagingApi", configuration.GetValue<string>("MessagingApiClient"));
//            clientUrls.Add("admin", configuration.GetValue<string>("SpaClient"));
//            clientUrls.Add("js", configuration.GetValue<string>("SpaClient"));

//            //add clients
//            foreach (var client in identityDataConfig.GetClients(clientUrls))
//            {
//                if (!context.Clients.Any(c => c.ClientId == client.ClientId))
//                    await context.Clients.AddAsync(client.ToEntity());
//            }
//            await context.SaveChangesAsync();

//            //add identity resources
//            foreach (var resource in identityDataConfig.GetResources())
//            {
//                if (!context.IdentityResources.Any(ir => ir.Name == resource.Name))
//                    await context.IdentityResources.AddAsync(resource.ToEntity());
//            }
//            await context.SaveChangesAsync();

//            //add api resources
//            foreach (var api in identityDataConfig.GetApis())
//            {
//                if (!context.ApiResources.Any(r => r.Name == api.Name))
//                    await context.ApiResources.AddAsync(api.ToEntity());
//            }
//            await context.SaveChangesAsync();
//        }
//    }
//}
