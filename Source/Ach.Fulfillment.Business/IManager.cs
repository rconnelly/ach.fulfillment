namespace Ach.Fulfillment.Business
{
    using System;
    using System.Collections.Generic;

    using Ach.Fulfillment.Data.Common;

    [Obsolete("do we really need it??")]
    public interface IManager<T>
    {
        T Create(T instance);

        void Update(T instance, bool flush = false);

        T Load(long id);

        IEnumerable<T> FindAll(IQueryData<T> queryData);

        int Count(IQueryData<T> queryData);

        T FindOne(IQueryData<T> queryData);

        void Delete(T instance);
    }
}