using Emeint.Core.BE.Configurations.Domain.Model;
using Emeint.Core.BE.Configurations.Infrastructure.Repositories;
using Emeint.Core.BE.Domain.Models;
using Emeint.Core.BE.InterCommunication;
using Emeint.Core.BE.Notifications.API.Filters;
using Emeint.Core.BE.Notifications.API.InterCommunication.Consumers;
using Emeint.Core.BE.Notifications.Domain.AggregatesModel.MessageAggregate;
using Emeint.Core.BE.Notifications.Domain.AggregatesModel.PlatformAggregate;
using Emeint.Core.BE.Notifications.Domain.AggregatesModel.UserAggregate;
using Emeint.Core.BE.Notifications.Domain.Configurations;
using Emeint.Core.BE.Notifications.Domain.Manager;
using Emeint.Core.BE.Notifications.Domain.Manager.Concretes;
using Emeint.Core.BE.Notifications.Domain.Manager.Contracts;
using Emeint.Core.BE.Notifications.Infrastructure.Repositories;
using Emeint.Core.BE.Notifications.Services.PushNotificationProviders;
using Emeint.Core.BE.Notifications.Services.PushNotificationProviders.Azure;
using Emeint.Core.BE.Notifications.Services.PushNotificationProviders.FireBase;
using Emeint.Core.BE.Notifications.Services.Serivces.Concretes;
using Emeint.Core.BE.Notifications.Services.Serivces.Contracts;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace Emeint.Core.BE.Notifications
{
    public class NotificationDependenciesResolver
    {

        public static void Register(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<SQLConnectionStringOptions>();
            #region Add IFileProvider
            services.AddSingleton<IFileProvider>(
                new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));
            #endregion
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IMessageStatusRepository, MessageStatusRepository>();
            services.AddScoped<IMessageTypeRepository, MessageTypeRepository>();
            services.AddScoped<IMessageTemplateRepository, MessageTemplateRepository>();
            services.AddScoped<IDevicePlatformRepository, DevicePlatformRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserDeviceRepository, UserDeviceRepository>();
            services.AddScoped<WhiteListFilter>();
            services.AddScoped<ISettingRepository, SettingRepository>();
            services.AddScoped<IConfigurationManager, Emeint.Core.BE.Notifications.Domain.Configurations.ConfigurationManager>();
            services.AddScoped<IHashingManager, HashingManager>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IMessageDestinationRepository, MessageDestinationRepository>();
            services.AddScoped<IMessageAggregateService, MessageAggregateService>();
            services.AddScoped<IAdminMessageService, AdminMessageService>();
            services.AddScoped<IEndUserMessageService, EndUserMessageService>();
            services.AddScoped<IServerMessageService, ServerMessageService>();

            if (configuration["PushNotificationProvider"]?.ToLower() == "azure")
                services.AddScoped<IMobilePushNotificationProvider, AzureMobilePushNotificationProvider>();
            else if (configuration["PushNotificationProvider"]?.ToLower() == "firebase")
                services.AddScoped<IMobilePushNotificationProvider, FirebaseMobileNotificationProvider>();

            services.AddScoped<IWebBrowserPushNotificationProvider, FirebaseWebBrowserNotificationProvider>();

            #region Register Consumers
            services.AddScoped<SendNotificationQMessageConsumer>();
            services.AddScoped<RegisterUserQMessageConsumer>();

            InterCommDependenciesConfigurator.ConfigureServices(services, configuration, (provider, cfg) =>
            {
                cfg.ReceiveEndpoint("ServiceRequired.SendPushNotification", e => { e.Consumer<SendNotificationQMessageConsumer>(provider); });
                cfg.ReceiveEndpoint("EntityChanges.UserToMessaging", e => { e.Consumer<RegisterUserQMessageConsumer>(provider); });
            });

            #endregion

        }
    }
}