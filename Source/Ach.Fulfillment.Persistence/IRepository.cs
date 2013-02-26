namespace Ach.Fulfillment.Persistence
{
    using System.Linq;

    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Common;

    public interface IRepository
    {
        IQueryable<T> Query<T>(IQueryData queryData);

        T Scalar<T>(IQueryData queryData);

        int Count<T>(IQueryData queryData);

        void Execute(IActionData actionData);

        void Flush(bool force = false);
    }
}