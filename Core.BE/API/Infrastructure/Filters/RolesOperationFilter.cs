using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Emeint.Core.BE.API.Infrastructure.Filters
{
    public class RolesOperationFilter : Swashbuckle.AspNetCore.SwaggerGen.IOperationFilter
    {

        private static void AppendRoles(Microsoft.AspNetCore.Authorization.AuthorizeAttribute authorizeAttributes, StringBuilder authorizationDescription)
        {
            var roles = authorizeAttributes.Roles;

            if (roles != null && roles.Any())
            {
                authorizationDescription.Append(roles);
            }
            else
            {
                authorizationDescription.Append("Any");
            }
        }


        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var attributes = context.ApiDescription.CustomAttributes();

            var authAttribute = attributes.FirstOrDefault(a => a is Microsoft.AspNetCore.Authorization.AuthorizeAttribute);

            if (authAttribute != null)
            {
                var authorizationDescription = new StringBuilder(" Allowed Roles: ");
                AppendRoles((Microsoft.AspNetCore.Authorization.AuthorizeAttribute)authAttribute, authorizationDescription);

                if (operation.Description == null)
                    operation.Description = string.Empty;

                operation.Description += authorizationDescription.ToString();
            }
        }
    }
}
