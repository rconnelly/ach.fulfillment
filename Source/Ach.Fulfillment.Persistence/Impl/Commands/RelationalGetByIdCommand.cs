namespace Ach.Fulfillment.Persistence.Impl.Commands
{
    using System.Diagnostics.Contracts;
    using System.Linq;

    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Common;

    internal class RelationalGetByIdCommand<TEntity> :
        RelationalQueryCommand<CommonGetQueryData<TEntity>, TEntity>
        where TEntity : class, IIdentified
    {
        #region Public Methods and Operators

        public override IQueryable<TEntity> Execute(CommonGetQueryData<TEntity> queryData)
        {
            var instance = this.ExecuteScalar(queryData);
            var result = (instance != null ? new[] { instance } : new TEntity[0]).AsQueryable();
            return result;
        }

        public override TEntity ExecuteScalar(CommonGetQueryData<TEntity> queryData)
        {
            Contract.Assert(queryData != null);
            var instance = this.Session.Get<TEntity>(queryData.Key);
            return instance;
        }

        #endregion
    }
}