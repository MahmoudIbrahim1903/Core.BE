using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Cache.Services.Contracts
{
    public interface ICacheService
    {
        /// <summary>
        /// Add/Update an item to/from the Default cache region.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        bool SaveItemToCache<T>(string itemKey, T value);

        /// <summary>
        /// Retrives an item from the Default cache region
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        T GetItemFromCache<T>(string itemKey);

        /// <summary>
        /// Deletes an item from the Default cache region
        /// </summary>
        /// <param name="key"></param>
        bool DeleteItemFromCache(string itemKey);

        /// <summary>
        /// Add/Update an item to/from specific region. In case the region is not exists, 
        /// it will be created and the item will be saved on it.
        /// </summary>
        /// <param name="regionKey"></param>
        /// <param name="itemKey"></param>
        /// <param name="value"></param>
        bool SaveItemToCacheRegion<T>(string regionKey, string itemKey, T value);

        /// <summary>
        /// Deletes an item from specific cache region
        /// </summary>
        /// <param name="regionKey"></param>
        /// <param name="itemKey"></param>
        bool DeleteItemFromCacheRegion(string regionKey, string itemKey);

        /// <summary>
        /// Retrieves an item from cache region
        /// </summary>
        /// <param name="regionKey"></param>
        /// <param name="itemKey"></param>
        /// <returns></returns>
        T GetItemFromCacheRegion<T>(string regionKey, string itemKey);

        bool SaveItemToCache<T>(string itemKey, T value, TimeSpan absoluteExpiration);
        bool SaveItemToCacheRegion<T>(string regionKey, string itemKey, T value, TimeSpan absoluteExpiration);

        /// <summary>
        /// Deletes all items of cache region
        /// </summary>
        /// <param name="regionKey"></param>
        void ClearCacheRegion(string regionKey);

        /// <summary>
        /// Clear all cache regions
        /// </summary>
        void ResetCache();
    }
}
