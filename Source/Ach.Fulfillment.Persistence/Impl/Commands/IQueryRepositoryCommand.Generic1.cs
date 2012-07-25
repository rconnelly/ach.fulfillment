namespace Ach.Fulfillment.Persistence.Impl.Commands
{
    using System.Linq;

    using Ach.Fulfillment.Data.Common;

    internal interface IQueryRepositoryCommand<out TResult>
    {
        IQueryable<TResult> FindAll(IQueryData queryData);

        TResult FindOne(IQueryData queryData);

        int RowCount(IQueryData queryData);
    }
}