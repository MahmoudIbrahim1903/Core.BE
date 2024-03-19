using Emeint.Core.BE.Domain.Exceptions;
using Emeint.Core.BE.Identity.Domain.Exceptions;
using Emeint.Core.BE.Identity.Infrastructure.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Identity.Infrastructure.Services.Concretes
{
    public class ClientRoleValidator : IClientRoleValidator
    {
        public Dictionary<string, List<string>> ClientsRoles = new Dictionary<string, List<string>>
        {
            {"Admin Portal", new List<string> {
                                                "SuperAdmin",
                                                "ConfigurationsReader","ConfigurationsFullController",
                                                "VersionsReader", "VersionsFullController",
                                                "AdministratorsReader", "AdministratorsFullController",
                                                "NotificationsReader","NotificationsFullController"
                                              }
            }

        };
        public virtual void ValidateClientRole(string clientName, List<string> userRoles)
        {
            if (clientName == null)
                throw new MissingParameterException("client");

            var clientRoles = ClientsRoles[clientName];
            var allowedRoles = clientRoles.Intersect(userRoles);
            if (clientRoles == null || allowedRoles == null || !allowedRoles.Any())
                throw new InvalidRoleException(clientName.ToLower(), clientName.ToLower());
        }

    }
}
