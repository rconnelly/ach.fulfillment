using System;

namespace Ach.Fulfillment.Web.Common
{
    using System.Diagnostics.Contracts;
    using System.Runtime.Caching;

    public static class CacheHelper
    {
        private static readonly ObjectCache Cache = MemoryCache.Default;

        private static readonly object Lock = new object();

        public static T GetOrAdd<T>(string key, Func<T> get)
            where T : class
        {
            Contract.Assert(key != null);
            Contract.Assert(get != null);

            var item = default (T);

            if (Cache.Contains(key))
            {
                lock (Lock)
                {
                    if (Cache.Contains(key))
                    {
                        item = Cache[key] as T;
                    }
                }
            }

            if(item == null)
            {
                item = get();
                Cache.AddOrGetExisting(key, item, DateTime.Now.AddDays(1));
            }

            return item;
        }

        public static void Remove(string key)
        {
            if (Cache.Contains(key))
            {
                Cache.Remove(key);
            }
        }
    }
}