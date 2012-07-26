namespace Ach.Fulfillment.Persistence.Impl.Commands
{
    using System.Linq;

    using Ach.Fulfillment.Data.Common;

    internal interface IQueryRepositoryCommand<out TResult>
    {
        IQueryable<TResult> Execute(IQueryData queryData);

        int RowCount(IQueryData queryData);
    }
}