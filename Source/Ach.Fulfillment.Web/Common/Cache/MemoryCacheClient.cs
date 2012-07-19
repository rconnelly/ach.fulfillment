namespace Ach.Fulfillment.Web.Common.Cache
{
    using System;
    using System.Runtime.Caching;

    using Ach.Fulfillment.Web.Properties;

    public class MemoryCacheClient : ICacheClient
    {
        private static readonly ObjectCache Cache = MemoryCache.Default;

        private static readonly object Lock = new object();

        public T Get<T>(string key)
        {
            T item;

            this.TryGet(key, out item);

            return item;
        }

        private bool TryGet<T>(string key, out T t)
        {
            var found = false;
            t = default(T);

            if (Cache.Contains(key))
            {
                lock (Lock)
                {
                    if (Cache.Contains(key))
                    {
                        t = (T)Cache[key];
                        found = true;
                    }
                }
            }

            return found;
        }

        public T GetOrAdd<T>(string key, Func<T> get)
        {
            T item;

            var found = this.TryGet(key, out item);

            if (!found)
            {
                item = get();
                Cache.AddOrGetExisting(key, item, DateTime.Now.Add(Settings.Default.CacheExpiration));
            }

            return item;
        }

        public void Remove(string key)
        {
            if (Cache.Contains(key))
            {
                Cache.Remove(key);
            }
        }
    }
}