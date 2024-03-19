using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Chatting.Domain.Configurations
{
    public interface IConfigurationManager
    {
        string GetSendbirdToken();
        string GetSendbirdAppId();
        string GetChattingAdminId();
        string GetChattingAdminName();

    }
}
