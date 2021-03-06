namespace Ach.Fulfillment.Persistence.Impl.Commands
{
    using System.Diagnostics.Contracts;

    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Common;

    internal class RelationalGetByIdCommand<TEntity> :
        RelationalScalarQueryCommand<CommonGetQueryData<TEntity>, TEntity>
        where TEntity : class, IIdentified
    {
        #region Public Methods and Operators

        public override TEntity ExecuteScalar(CommonGetQueryData<TEntity> queryData)
        {
            Contract.Assert(queryData != null);
            var instance = this.Session.Get<TEntity>(queryData.Key);
            return instance;
        }

        #endregion
    }
}