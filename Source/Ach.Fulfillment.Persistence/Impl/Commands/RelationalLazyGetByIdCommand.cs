namespace Ach.Fulfillment.Persistence.Impl.Commands
{
    using System.Diagnostics.Contracts;

    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Common;

    internal class RelationalLazyGetByIdCommand<TEntity> :
        RelationalScalarQueryCommand<CommonLazyGetQueryData<TEntity>, TEntity>
        where TEntity : class, IIdentified
    {
        #region Public Methods and Operators

        public override TEntity ExecuteScalar(CommonLazyGetQueryData<TEntity> queryData)
        {
            Contract.Assert(queryData != null);
            var instance = this.Session.Load<TEntity>(queryData.Key);
            return instance;
        }

        #endregion
    }
}