namespace Ach.Fulfillment.Persistence.Impl.Commands
{
    using System.Linq;

    using Ach.Fulfillment.Data.Common;

    internal interface IQueryCommand<out TResult>
    {
        IQueryable<TResult> Execute(IQueryData queryData);

        TResult ExecuteScalar(IQueryData queryData);

        int RowCount(IQueryData queryData);
    }
}