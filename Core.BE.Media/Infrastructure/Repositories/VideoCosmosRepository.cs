using Emeint.Core.BE.API.Infrastructure.Services;
using Emeint.Core.BE.Infrastructure.Repositories;
using Emeint.Core.BE.Media.Domain.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Media.Infrastructure.Repositories
{
    public class VideoCosmosRepository : BaseCosmosRepository<Video>, IVideoRepository
    {
        public VideoCosmosRepository(CosmosClient cosmosClient, IConfiguration configuration, IIdentityService identityService) : base(cosmosClient, configuration, "Videos", identityService)
        {
        }

        public async Task DeleteVideoByCode(string videoCode)
        {
            var video = GetAll().Where(d => d.Code == videoCode).ToList()?.FirstOrDefault();
            if (video != null)
                await DeleteAsync(video);
        }

        public Video GetVideoByCode(string videoCode)
        {
            return GetAll().Where(d => d.Code == videoCode).ToList()?.FirstOrDefault();
        }
    }
}
