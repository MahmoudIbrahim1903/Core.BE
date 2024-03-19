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
    public class VideoRepository : BaseRepository<Video, MediaContext>, IVideoRepository
    {
        public VideoRepository(MediaContext context) : base(context)
        {
        }

        public async Task<Video> AddAsync(Video entity)
        {
            var video = base.Add(entity);
            await _context.SaveChangesAsync();
            return video;
        }

        public async Task DeleteVideoByCode(string videoCode)
        {
            var video = _context.Videos.FirstOrDefault(v => v.Code == videoCode);
            _context.Videos.Remove(video);
            await _context.SaveChangesAsync();
        }

        public Video GetVideoByCode(string videoCode)
        {
            return _context.Videos.FirstOrDefault(v => v.Code == videoCode);
        }

        async Task IAsyncRepository<Video>.DeleteAsync(Video entity)
        {
            base.Delete(entity);
            await _context.SaveChangesAsync();
        }

        async Task IAsyncRepository<Video>.UpdateAsync(Video entity)
        {
            base.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
