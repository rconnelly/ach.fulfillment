namespace Ach.Fulfillment.Persistence.Impl.Commands
{
    using System.Linq;

    using Ach.Fulfillment.Data.Common;

    internal interface IQueryCommand<in TQueryData, out TResult> : IQueryCommand<TResult>
        where TQueryData : IQueryData
    {
        IQueryable<TResult> Execute(TQueryData queryData);

        TResult ExecuteScalar(TQueryData queryData);

        int RowCount(TQueryData queryData);
    }
}