namespace Emeint.Core.BE.API.Controllers
{
    //[Route("/api/portal/{Controller}/")]
    public class BasePortalApiController<T> : BaseApiController<T> where T : new()
    {
    }
}
