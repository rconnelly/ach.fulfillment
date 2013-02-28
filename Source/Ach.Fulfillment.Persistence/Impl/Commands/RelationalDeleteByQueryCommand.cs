namespace Ach.Fulfillment.Persistence.Impl.Commands
{
    using System;
    using System.Diagnostics.Contracts;

    using Ach.Fulfillment.Data.Common;
    using Ach.Fulfillment.Persistence.Impl.Configuration;

    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
    using Microsoft.Practices.Unity;

    using NHibernate;

    internal class RelationalDeleteByQueryCommand<TData> : ActionCommandBase<TData>
        where TData : class, IDeleteByQueryActionData
    {
        #region Public Properties

        [Dependency]
        public ISession Session { get; set; }

        #endregion

        #region Public Methods and Operators

        public override void Execute(TData queryData)
        {
            Contract.Assert(queryData != null);
            try
            {
                this.Session.Delete(queryData.Query);
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
