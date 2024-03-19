using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.InterCommunication.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Mailing.Domain.Managers
{
    public interface IEmailManager
    {
        Task SendEmail(string from, string fromDisplayName, string[] to, string[] cc, string subject, string body, List<AttachmentVM> attachments,
            bool? isImportant, string type, Dictionary<string, string> extraParams);
    }
}
