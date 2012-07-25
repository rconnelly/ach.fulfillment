namespace Ach.Fulfillment.Persistence.Impl.Commands
{
    using System.Collections.Generic;
    using System.Linq;

    using Ach.Fulfillment.Data.Common;

    internal interface IQueryRepositoryCommand<out TResult>
    {
        IEnumerable<TResult> FindAll(IQueryData queryData);

        IQueryable<TResult> FindQuery(IQueryData queryData);

        TResult FindOne(IQueryData queryData);

        int RowCount(IQueryData queryData);
    }
}