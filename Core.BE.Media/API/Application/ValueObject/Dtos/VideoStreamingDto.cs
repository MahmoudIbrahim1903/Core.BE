using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Media.API.Application.ValueObject.Dtos
{
    public class VideoStreamingDto
    {
        public string Url { set; get; }
        public string AesToken { set; get; }
    }
}
