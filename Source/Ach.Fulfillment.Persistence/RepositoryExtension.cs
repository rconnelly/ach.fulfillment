// ReSharper disable CheckNamespace
namespace Ach.Fulfillment.Data
// ReSharper restore CheckNamespace
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;

    using Ach.Fulfillment.Common.Exceptions;
    using Ach.Fulfillment.Data.Common;
    using Ach.Fulfillment.Persistence;

    public static class RepositoryExtension
    {
        #region Methods

        public static void Create<T>(this IRepository repository, T instance)
        {
            Contract.Assert(repository != null);
            repository.Execute(new CommonCreateActionData<T> { Instance = instance });
        }

        public static void Update<T>(this IRepository repository, T instance)
        {
            Contract.Assert(repository != null);
            repository.Execute(new CommonUpdateActionData<T> { Instance = instance });
        }

        public static void Delete<T>(this IRepository repository, T instance)
        {
            Contract.Assert(repository != null);
            repository.Execute(new CommonDeleteActionData<T> { Instance = instance });
        }

        public static T Get<T>(this IRepository repository, long id)
        {
            return repository.FindOne(new CommonGetQueryData<T> { Key = id });
        }

        public static T Load<T>(this IRepository repository, long id)
            where T : class
        {
            return repository.LoadOne(new CommonGetQueryData<T> { Key = id });
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "As Designed")]
        public static int Count<T>(this IRepository repository, IQueryData<T> queryData)
        {
            Contract.Assert(repository != null);
            return repository.Count<T>(queryData);
        }

        public static IEnumerable<T> FindAll<T>(this IRepository repository, IQueryData<T> queryData)
        {
            Contract.Assert(repository != null);
            return repository.FindAll<T>((IQueryData)queryData);
        }

        public static IEnumerable<T> FindAll<T>(this IRepository repository, IQueryData queryData)
        {
            Contract.Assert(queryData != null);
            var query = repository.Query<T>(queryData);
            var result = query.ToList();
            return result;
        }

        public static T LoadOne<T>(this IRepository repository, IQueryData<T> queryData)
            where T : class
        {
            Contract.Assert(repository != null);
            Contract.Assert(queryData != null);
            var instance = repository.FindOne(queryData);
            if (instance == null)
            {
                var message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Cannot load [{0}] using criteria [{1}].",
                    typeof(T),
                    queryData);
                throw new ObjectNotFoundException(message);
            }

            return instance;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "As Designed")]
        public static T FindOne<T>(this IRepository repository, IQueryData<T> queryData)
        {
            Contract.Assert(repository != null);
            return repository.FindOne<T>((IQueryData)queryData);
        }

        public static T FindOne<T>(this IRepository repository, IQueryData queryData)
        {
            Contract.Assert(queryData != null);
            var result = repository.Scalar<T>(queryData);
            return result;
        }

        public static IQueryable<T> Query<T>(this IRepository repository, IQueryData<T> queryData)
        {
            Contract.Assert(repository != null);
            return repository.Query<T>(queryData);
        }

        #endregion
    }
}