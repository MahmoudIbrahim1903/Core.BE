using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Media.Domain.Enums
{
    public enum MediaServicesEncodingJobStatus
    {
        Canceled,
        Canceling,
        Error,
        Finished,
        Processing,
        Queued,
        Scheduled
    }
}
