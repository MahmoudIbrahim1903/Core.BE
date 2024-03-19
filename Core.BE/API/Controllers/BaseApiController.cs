using Microsoft.AspNetCore.Mvc;

namespace Emeint.Core.BE.API.Controllers
{
    // [ValidateModelState]
    public class BaseApiController<T> : Controller where T : new()
    {
        //protected BaseManager<T> _manager = new BaseManager<T>();
    }
}
