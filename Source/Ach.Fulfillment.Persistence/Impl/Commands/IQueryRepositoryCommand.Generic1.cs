namespace Ach.Fulfillment.Persistence.Impl.Commands
{
    using System.Collections.Generic;

    using Ach.Fulfillment.Data.Common;

    internal interface IQueryRepositoryCommand<out TResult>
    {
        IEnumerable<TResult> FindAll(IQueryData queryData);

        TResult FindOne(IQueryData queryData);

        int RowCount(IQueryData queryData);
    }
}