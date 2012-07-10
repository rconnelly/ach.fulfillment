namespace Ach.Fulfillment.Persistence.Impl.Commands
{
    using System.Collections.Generic;

    using Ach.Fulfillment.Data.QueryData;

    internal interface IQueryRepositoryCommand<in TQueryData, out TResult> : IQueryRepositoryCommand<TResult>
        where TQueryData : IQueryData
    {
        IEnumerable<TResult> FindAll(TQueryData queryData);

        TResult FindOne(TQueryData queryData);

        int RowCount(TQueryData queryData);
    }
}