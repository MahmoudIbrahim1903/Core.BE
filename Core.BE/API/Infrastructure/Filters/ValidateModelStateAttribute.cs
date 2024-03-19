using Emeint.Core.BE.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace Emeint.Core.BE.API.Infrastructure.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited =true, AllowMultiple = true)]
    public class ValidateModelStateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var error = string.Join("; ", context.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));

                throw new InvalidParameterException("model state", "", error);
            }
        }
    }
}