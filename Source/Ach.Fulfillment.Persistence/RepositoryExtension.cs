// ReSharper disable CheckNamespace
namespace Ach.Fulfillment.Data
// ReSharper restore CheckNamespace
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Globalization;

    using Ach.Fulfillment.Common.Exceptions;
    using Ach.Fulfillment.Data.Common;
    using Ach.Fulfillment.Persistence;

    public static class RepositoryExtension
    {
        public static int Count<T>(this IRepository repository, IQueryData<T> queryData)
        {
            Contract.Assert(repository != null);
            return repository.Count<T>(queryData);
        }

        public static IEnumerable<T> FindAll<T>(this IRepository repository, IQueryData<T> queryData)
        {
            Contract.Assert(repository != null);
            return repository.FindAll<T>(queryData);
        }

        public static T FindOne<T>(this IRepository repository, IQueryData<T> queryData)
        {
            Contract.Assert(repository != null);
            return repository.FindOne<T>(queryData);
        }

        public static T LoadOne<T>(this IRepository repository, IQueryData<T> queryData)
            where T : class
        {
            Contract.Assert(repository != null);
            Contract.Assert(queryData != null);
            var instance = repository.FindOne<T>(queryData);
            if (instance == null)
            {
                var message = string.Format(
                    CultureInfo.InvariantCulture, 
                    "Cannot load [{0}] using criteria [{1}].",
                    typeof(T),
                    queryData.GetType());
                throw new ObjectNotFoundException(message);
            }

            return instance;
        }
    }
}