using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Chatting.Services.Contracts
{
    public interface IUserService
    {

        List<string> GetUsersTypes(bool excludeAdmin);
    }
}
