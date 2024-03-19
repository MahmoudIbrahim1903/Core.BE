using Emeint.Core.BE.Domain.SeedWork;
using Emeint.Core.BE.Media.Domain.Enums;

namespace Emeint.Core.BE.Media.Domain.Models
{
    public class Video : Entity
    {
        public string FileName { get; set; }
        public string Url { get; set; }
        public MediaServicesEncodingJobStatus? EncodingJobStatus { set; get; }
        public string StreamingUrl { get; set; }
    }
}
