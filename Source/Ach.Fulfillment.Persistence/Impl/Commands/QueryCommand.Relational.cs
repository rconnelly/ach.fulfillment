namespace Ach.Fulfillment.Persistence.Impl.Commands
{
    using Ach.Fulfillment.Data.Common;

    using Microsoft.Practices.Unity;

    using NHibernate;

    internal abstract class RelationalQueryCommand<TQueryData, TResult> : QueryCommandBase<TQueryData, TResult>
        where TQueryData : IQueryData<TResult>
    {
        #region Public Properties

        [Dependency]
        public ISession Session { get; set; }

        #endregion
    }
}