using Emeint.Core.BE.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Media.Domain.Models
{
    public interface IAudioRepository : IAsyncRepository<Audio>
    {
        Audio GetAudioByCode(string audioCode);
    }
}
