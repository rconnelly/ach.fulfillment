namespace Ach.Fulfillment.Persistence.Impl.Commands
{
    using System.Collections.Generic;

    using Ach.Fulfillment.Data.Common;

    internal interface IQueryCommand<out TResult>
    {
        IEnumerable<TResult> Execute(IQueryData queryData);

        TResult ExecuteScalar(IQueryData queryData);

        int RowCount(IQueryData queryData);
    }
}