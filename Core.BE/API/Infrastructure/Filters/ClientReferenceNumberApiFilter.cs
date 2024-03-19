using Microsoft.AspNetCore.Mvc.Filters;

namespace Emeint.Core.BE.API.Infrastructure.Filters
{
    class ClientReferenceNumberApiFilter : ActionFilterAttribute
    {

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // do something before the action executes

        }
    }
}
