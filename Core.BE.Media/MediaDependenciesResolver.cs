using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Media.API.Application.Handlers;
using Emeint.Core.BE.Media.API.Extensions;
using Emeint.Core.BE.Media.Domain.Configurations;
using Emeint.Core.BE.Media.Domain.Enums;
using Emeint.Core.BE.Media.Domain.Models;
using Emeint.Core.BE.Media.Infrastructure;
using Emeint.Core.BE.Media.Infrastructure.Repositories;
using Emeint.Core.BE.Media.Managers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConfigurationManager = Emeint.Core.BE.Media.Domain.Configurations.ConfigurationManager;

namespace Emeint.Core.BE.Media
{
    public static class MediaDependenciesResolver
    {
        public static void Register(IServiceCollection services, IConfiguration configuration)
        {
            services.AddImageDownloader();

            if (configuration["DataBaseType"] == "Cosmos")
            {
                services.AddScoped<IImageRepository, ImageCosmosRepository>();
                services.AddScoped<IVideoRepository, VideoCosmosRepository>();
                services.AddScoped<IAudioRepository, AudioCosmosRepository>();
                services.AddScoped<IDocumentRepository, DocumentCosmosRepository>();
            }
            else
            {
                services.AddDbContext<MediaContext>();
                services.AddScoped<IImageRepository, ImageRepository>();
                services.AddScoped<IVideoRepository, VideoRepository>();
                services.AddScoped<IAudioRepository, AudioRepository>();
                services.AddScoped<IDocumentRepository, DocumentRepository>();
            }

            services.AddScoped<IAzureMediaServicesManager, AzureMediaServicesManager>();
            services.AddScoped<IConfigurationManager, ConfigurationManager>();
            services.AddScoped<IExportExcelManager, ExportExcelManager>();
            services.AddScoped<IExportCsvManager, ExportCsvManager>();
            services.AddScoped<IExcelFileManager, ExcelFileManager>();
            services.AddScoped<ImageUploadHandler>();
            var parsedSuccessfully = Enum.TryParse(configuration["MediaStorageLocation"], out MediaStorageLocations mediaStorageLocation);
            if (!parsedSuccessfully)
                mediaStorageLocation = MediaStorageLocations.LocalStorage;

            if (mediaStorageLocation == MediaStorageLocations.LocalStorage)
                services.AddScoped<IMediaStorageManager, LocalMediaStorageManager>();
            else if (mediaStorageLocation == MediaStorageLocations.AzureStorage)
                services.AddScoped<IMediaStorageManager, AzureMediaStorageManager>();

        }
    }
}
