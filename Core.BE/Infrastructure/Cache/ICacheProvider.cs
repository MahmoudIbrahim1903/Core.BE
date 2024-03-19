namespace Emeint.Core.BE.Infrastructure.Cache
{
    public interface ICacheProvider
    {
        /// <summary>
        /// Get the object from the cash by an identified key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object Get(string key);

        /// <summary>
        /// Save the object in the cash by an identified key for a period of time.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data">the object to be cashed</param>
        /// <param name="cacheTime">in minutes</param>
        void Set(string key, object data, int cacheTime);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool IsSet(string key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        void Invalidate(string key);
    }
}
