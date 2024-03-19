using System;
using System.Linq;

namespace Emeint.Core.BE.Infrastructure.Cache
{
    public class CacheProvider : Singleton<CacheProvider>, ICacheProvider
    {

        //private static CacheProvider instance;
        private CacheProvider() { }

        //public static CacheProvider Instance
        //{
        //    get { return instance ?? (instance = new CacheProvider()); }
        //}

        private ObjectCache Cache { get { return MemoryCache.Default; } }

        public object Get(string key)
        {
            return Cache[GetServiceKey(key)];

        }

        private string GetServiceKey(string key)
        {
            return key + Identity.Instance.ServiceId;
        }

        public void Set(string key, object data, int cacheTime)
        {
            var policy = new CacheItemPolicy { AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(cacheTime) };

            Cache.Add(new CacheItem(GetServiceKey(key), data), policy);

        }

        public bool IsSet(string key)
        {
            return (Cache[GetServiceKey(key)] != null);

        }

        public void Invalidate(string key)
        {

            var cacheItems = (from n in Cache.AsParallel() select n).ToList();

            foreach (var a in cacheItems)
                Cache.Remove(a.Key);

            // Cache.Remove(key);

        }
    }
}