namespace Ach.Fulfillment.Persistence.Impl.Commands
{
    using System.Diagnostics.Contracts;

    using Ach.Fulfillment.Data.Common;

    using Microsoft.Practices.Unity;

    using NHibernate;

    internal class RelationalCreateCommand<TEntity> : ActionCommandBase<CommonCreateActionData<TEntity>>
    {
        #region Public Properties

        [Dependency]
        public ISession Session { get; set; }

        #endregion

        #region Public Methods and Operators

        public override void Execute(CommonCreateActionData<TEntity> queryData)
        {
            Contract.Assert(queryData != null);
            this.Session.Save(queryData.Instance);
        }

        #endregion
    }
}