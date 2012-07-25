namespace Ach.Fulfillment.Persistence.Impl.Commands
{
    using System.Linq;

    using Ach.Fulfillment.Data.Common;

    using Microsoft.Practices.Unity;

    using NHibernate;

    internal abstract class CommandBase<TQueryData, TResult> : IQueryRepositoryCommand<TQueryData, TResult>
        where TQueryData : IQueryData<TResult>
    {
        #region Public Properties

        [Dependency]
        public ISession Session { get; set; }

        #endregion

        #region Public Methods and Operators

        public abstract IQueryable<TResult> FindAll(TQueryData queryData);
        
        public abstract TResult FindOne(TQueryData queryData);

        public abstract int RowCount(TQueryData queryData);

        #endregion

        #region Explicit Interface Methods

        IQueryable<TResult> IQueryRepositoryCommand<TResult>.FindAll(IQueryData queryData)
        {
            return this.FindAll((TQueryData)queryData);
        }

        TResult IQueryRepositoryCommand<TResult>.FindOne(IQueryData queryData)
        {
            return this.FindOne((TQueryData)queryData);
        }

        int IQueryRepositoryCommand<TResult>.RowCount(IQueryData queryData)
        {
            return this.RowCount((TQueryData)queryData);
        }

        #endregion
    }
}