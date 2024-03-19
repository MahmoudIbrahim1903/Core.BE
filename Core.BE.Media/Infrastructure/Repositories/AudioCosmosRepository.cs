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
    public class AudioCosmosRepository : BaseCosmosRepository<Audio>, IAudioRepository
    {
        public AudioCosmosRepository(CosmosClient cosmosClient, IConfiguration configuration, IIdentityService identityService) : base(cosmosClient, configuration, "Audios", identityService)
        {
        }

        public Audio GetAudioByCode(string audioCode)
        {
            return GetAll().Where(c => c.Code == audioCode).ToList()?.FirstOrDefault();
        }
    }
}
