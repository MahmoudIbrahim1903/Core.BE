using Emeint.Core.BE.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Media.Domain.Models
{
    public interface IVideoRepository : IAsyncRepository<Video>
    {
        Video GetVideoByCode(string videoCode);
        Task DeleteVideoByCode(string videoCode);
    }
}
