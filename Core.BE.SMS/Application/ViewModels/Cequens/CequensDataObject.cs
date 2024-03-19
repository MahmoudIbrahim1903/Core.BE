using System.Collections.Generic;

namespace Emeint.Core.BE.SMS.Application.ViewModels.Cequens
{
    public class CequensDataObject
    {
        public List<CequensSentSmsIdsObject> SentSMSIDs { get; set; }
        public string InvalidRecipients { get; set; }

        public List<object> Errors { get; set; }
    }
}
