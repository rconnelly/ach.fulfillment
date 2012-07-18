using System;

namespace Ach.Fulfillment.Web.Common
{
    using System.Diagnostics.Contracts;
    using System.Runtime.Caching;

    using Ach.Fulfillment.Web.Properties;

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
                Cache.AddOrGetExisting(key, item, DateTime.Now.Add(Settings.Default.CacheExpiration));
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