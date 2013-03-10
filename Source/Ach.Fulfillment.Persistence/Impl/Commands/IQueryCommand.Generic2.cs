namespace Ach.Fulfillment.Persistence.Impl.Commands
{
    using System.Collections.Generic;

    using Ach.Fulfillment.Data.Common;

    internal interface IQueryCommand<in TQueryData, out TResult> : IQueryCommand<TResult>
        where TQueryData : IQueryData
    {
        IEnumerable<TResult> Execute(TQueryData queryData);

        TResult ExecuteScalar(TQueryData queryData);

        int RowCount(TQueryData queryData);
    }
}