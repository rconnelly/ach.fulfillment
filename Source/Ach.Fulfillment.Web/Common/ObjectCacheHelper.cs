namespace Ach.Fulfillment.Web.Common
{
    using System;
    using System.Runtime.Caching;

    using Ach.Fulfillment.Web.Properties;

    internal static class ObjectCacheHelper
    {
        public static T GetOrAdd<T>(this ObjectCache cache, string key, Func<T> get)
            where T : class 
        {
            var item = (T)cache.Get(key);

            if (item == null)
            {
                item = get();
                if (item != null)
                {
                    cache.AddOrGetExisting(key, item, DateTime.Now.Add(Settings.Default.CacheExpiration));
                }
            }

            return item;
        }
    }
}