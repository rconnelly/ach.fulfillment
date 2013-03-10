namespace Ach.Fulfillment.Persistence
{
    using System.Collections.Generic;

    using Ach.Fulfillment.Data.Common;

    public interface IRepository
    {
        IEnumerable<T> Enumerable<T>(IQueryData queryData);

        T Scalar<T>(IQueryData queryData);

        int Count<T>(IQueryData queryData);

        void Execute(IActionData actionData, bool flush = false);
    }
}