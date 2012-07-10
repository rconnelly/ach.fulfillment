﻿namespace Ach.Fulfillment.Persistence
{
    using System.Collections.Generic;

    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.QueryData;

    public interface IRepository
    {
        void Create<T>(T instance) where T : class, IEntity;
        
        T Get<T>(long id) where T : class, IEntity;

        T Load<T>(long id) where T : class, IEntity;

        void Update<T>(T instance) where T : class, IEntity;

        void Delete<T>(T instance) where T : class, IEntity;

        int Count<T>(IQueryData queryData);

        IEnumerable<T> FindAll<T>(IQueryData queryData);

        T FindOne<T>(IQueryData specification);

        void Flush(bool force = false);
    }
}