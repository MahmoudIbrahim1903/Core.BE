using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Mailing
{
    public class AppSettings
    {
        public string ConnectionString { get; set; }
        public string IdentityUrl { get; set; }
        public bool UseCustomizationData { get; set; }
        //public int GracePeriodTime { get; set; }
        public string EventBusConnection { get; set; }
        public int CheckUpdateTime { get; set; }
    }
}
