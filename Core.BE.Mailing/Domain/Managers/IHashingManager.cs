using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emeint.Core.BE.Mailing.API.Application.ValueObjects.ViewModels;

namespace Emeint.Core.BE.Mailing.Domain.Managers
{
    public interface IHashingManager
    {
        string HashSendEmailRequest(SendMailViewModel mail);
    }
}
