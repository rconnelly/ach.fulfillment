namespace Ach.Fulfillment.Persistence.Impl.Commands
{
    using System.Diagnostics.Contracts;

    using Ach.Fulfillment.Data.Common;

    using Microsoft.Practices.Unity;

    using NHibernate;

    internal class RelationalUpdateCommand<TEntity> : ActionCommandBase<CommonUpdateActionData<TEntity>>
    {
        #region Public Properties

        [Dependency]
        public ISession Session { get; set; }

        #endregion

        #region Public Methods and Operators

        public override void Execute(CommonUpdateActionData<TEntity> queryData)
        {
            Contract.Assert(queryData != null);
            this.Session.Update(queryData.Instance);
        }

        #endregion
    }
}