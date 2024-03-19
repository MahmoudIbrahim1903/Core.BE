using Emeint.Core.BE.Media;

namespace Emeint.Core.BE.Identity
{
    public class AppSettings: ImageAppSettings
    {
        public string MvcClient { get; set; }

        public bool UseCustomizationData { get; set; }

        public string SendSmsBlockIntervalByMinutes { set; get; }

        public string SendEmailBlockIntervalByMinutes { set; get; }
    }
}
