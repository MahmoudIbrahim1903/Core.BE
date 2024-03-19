using Emeint.Core.BE.Domain.SeedWork;
using Emeint.Core.BE.Infrastructure.Repositories;
using Emeint.Core.BE.Media.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Media.Infrastructure.Repositories
{
    public class AudioRepository : BaseRepository<Audio, MediaContext>, IAudioRepository
    {
        public AudioRepository(MediaContext context) : base(context)
        {
        }

        public Audio GetAudioByCode(string audioCode)
        {
            return GetAll().FirstOrDefault(a => a.Code == audioCode);
        }
        public async Task<Audio> AddAsync(Audio entity)
        {
            base.Add(entity);
            await SaveEntitiesAsync();
            return entity;
        }

        async Task IAsyncRepository<Audio>.UpdateAsync(Audio entity)
        {
            base.Update(entity);
            await SaveEntitiesAsync();
        }

        async Task IAsyncRepository<Audio>.DeleteAsync(Audio entity)
        {
            base.Delete(entity);
            await SaveEntitiesAsync();
        }
    }
}
