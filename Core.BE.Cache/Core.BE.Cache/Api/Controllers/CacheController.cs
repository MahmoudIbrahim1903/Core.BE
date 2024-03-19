using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Emeint.Core.BE.API.Application.ValueObjects.ViewModels;
using Emeint.Core.BE.Cache.Services.Contracts;
using Emeint.Core.BE.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Emeint.Core.BE.Cache.Api.Controllers
{
    [Produces("application/json")]
    [ApiExplorerSettings(IgnoreApi = false)]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}")]
    public class CacheController : Controller
    {
        private ICacheService _cacheService;
        
        public CacheController(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        [Route("clear_region")]
        [HttpGet]
        public Task<Response<bool>> ClearRegion(string regionKey)
        {
            _cacheService.ClearCacheRegion(regionKey);
            return Task.FromResult(new Response<bool>() { Data = true, ErrorCode = (int)ErrorCodes.Success });
        }
        [Route("reset_cache")]
        [HttpGet]
        public Task<Response<bool>> ReSetCache()
        {
            _cacheService.ResetCache();
            return Task.FromResult(new Response<bool>() { Data = true, ErrorCode = (int)ErrorCodes.Success });
        }
        [Route("item")]
        [HttpGet]
        public Task<Response<object>> GetItem(string itemKey, string regionKey = "")
        {
            var item = string.IsNullOrEmpty(regionKey) ? _cacheService.GetItemFromCache<object>(itemKey) : 
                                                         _cacheService.GetItemFromCacheRegion<object>(regionKey, itemKey);
            return Task.FromResult(new Response<object> { Data = item, ErrorCode = (int)ErrorCodes.Success });
        }

        #region Commented - Testing Cache
        //public IActionResult Test_AddItemToCache(string itemKey, string value)
        //{
        //    if (string.IsNullOrEmpty(value))
        //    {
        //        _cacheService.SaveItemToCache(itemKey, new TestClass { First = "F I R S T", Second = "S E C O N D" });
        //        return Ok(_cacheService.GetItemFromCache(itemKey));
        //    }
        //    else
        //    {
        //        _cacheService.SaveItemToCache(itemKey, value);
        //        return Ok(_cacheService.GetItemFromCache(itemKey));
        //    }
        //}

        //public IActionResult Test_AddItemToCacheRegion(string itemKey, string value, string regionKey)
        //{
        //    if (string.IsNullOrEmpty(value))
        //    {
        //        _cacheService.SaveItemToCacheRegion(regionKey, itemKey, new TestClass { First = "F I R S T", Second = "S E C O N D" });
        //        return Ok(_cacheService.GetItemFromCache(itemKey));
        //    }
        //    else
        //    {
        //        _cacheService.SaveItemToCacheRegion(regionKey, itemKey, value);
        //        return Ok(_cacheService.GetItemFromCacheRegion(regionKey, itemKey));
        //    }
        //}

        //public IActionResult Test_AddItemToRegionWithExpiry(string itemKey, string value, string regionKey)
        //{
        //    TimeSpan timeSpan = new TimeSpan(0, 0, 5);
        //    if (string.IsNullOrEmpty(regionKey))
        //    {
        //        if (string.IsNullOrEmpty(value))
        //        {
        //            _cacheService.SaveItemToCache(itemKey, new TestClass { First = "F I R S T", Second = "S E C O N D" }, timeSpan);
        //            Thread.Sleep(timeSpan);
        //            return Ok(_cacheService.GetItemFromCache(itemKey));
        //        }
        //        else
        //        {
        //            _cacheService.SaveItemToCache(itemKey, value, timeSpan);
        //            Thread.Sleep(timeSpan);
        //            return Ok(_cacheService.GetItemFromCache(itemKey));
        //        }
        //    }
        //    else
        //    {
        //        if (string.IsNullOrEmpty(value))
        //        {
        //            _cacheService.SaveItemToCacheRegion(regionKey, itemKey, new TestClass { First = "F I R S T", Second = "S E C O N D" }, timeSpan);
        //            Thread.Sleep(timeSpan);
        //            return Ok(_cacheService.GetItemFromCacheRegion(regionKey, itemKey));
        //        }
        //        else
        //        {
        //            _cacheService.SaveItemToCacheRegion(regionKey, itemKey, value, timeSpan);
        //            var beforeExpiry = _cacheService.GetItemFromCacheRegion(regionKey, itemKey);
        //            Thread.Sleep(timeSpan);
        //            var item = _cacheService.GetItemFromCacheRegion(regionKey, itemKey);
        //            return Ok(item);
        //        }
        //    }
        //}

        //public IActionResult Test_ClearCacheReion(string region)
        //{
        //    _cacheService.ClearCacheRegion(region);
        //    return Ok("DONE!!");
        //}

        //public IActionResult Test_ResetCache()
        //{
        //    _cacheService.ReSetCache();
        //    return Ok("DONE!!");
        //}

        //class TestClass
        //{
        //    public string First { get; set; }
        //    public string Second { get; set; }
        //}
        #endregion Commented - Testing Cache
    }
}