namespace Emeint.Core.BE.API.Controllers
{
    //[Route("api/mobile/{controller}/{action}/{id}")]
    public class BaseMobileApiController<T> : BaseApiController<T> where T:  new()
    {
    }
}
