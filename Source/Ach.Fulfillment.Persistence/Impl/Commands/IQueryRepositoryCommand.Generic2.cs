namespace Ach.Fulfillment.Persistence.Impl.Commands
{
    using System.Linq;

    using Ach.Fulfillment.Data.Common;

    internal interface IQueryRepositoryCommand<in TQueryData, out TResult> : IQueryRepositoryCommand<TResult>
        where TQueryData : IQueryData
    {
        IQueryable<TResult> FindAll(TQueryData queryData);

        TResult FindOne(TQueryData queryData);

        int RowCount(TQueryData queryData);
    }
}