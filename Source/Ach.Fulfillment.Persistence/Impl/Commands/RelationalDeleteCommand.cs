namespace Ach.Fulfillment.Persistence.Impl.Commands
{
    using System;
    using System.Diagnostics.Contracts;

    using Ach.Fulfillment.Data.Common;
    using Ach.Fulfillment.Persistence.Impl.Configuration;

    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
    using Microsoft.Practices.Unity;

    using NHibernate;

    internal class RelationalDeleteCommand<TEntity> : ActionCommandBase<CommonDeleteActionData<TEntity>>
    {
        #region Public Properties

        [Dependency]
        public ISession Session { get; set; }

        #endregion

        #region Public Methods and Operators

        public override void Execute(CommonDeleteActionData<TEntity> queryData)
        {
            Contract.Assert(queryData != null);
            try
            {
                this.Session.Delete(queryData.Instance);
                this.Session.Flush();
            }
            catch (Exception ex)
            {
                var rethrow = ExceptionPolicy.HandleException(ex, PersistenceContainerExtension.DeletePolicy);
                if (rethrow)
                {
                    throw;
                }
            }
        }

        #endregion
    }
}