using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Gateway.Api.Filters
{
    public class HideOcelotControllersFilter : IDocumentFilter
    {
        private static readonly string[] _ignoredPaths = {
            "/configuration",
            "/outputcache/{region}"
        };

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (var ignorePath in _ignoredPaths)
            {
                swaggerDoc.Paths.Remove(ignorePath);
            }
        }
    }
}
