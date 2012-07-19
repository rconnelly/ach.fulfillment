namespace Ach.Fulfillment.Web.Common.Cache
{
    using System;

    public interface ICacheClient
    {
        T Get<T>(string key);

        T GetOrAdd<T>(string key, Func<T> get);

        void Remove(string key);
    }
}