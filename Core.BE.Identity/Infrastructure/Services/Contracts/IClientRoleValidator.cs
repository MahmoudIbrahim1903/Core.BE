using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.Infrastructure.Services.Contracts
{
    public interface IClientRoleValidator
    {
        void ValidateClientRole(string client, List<string> userRoles);
    }
}
