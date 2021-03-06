namespace Ach.Fulfillment.Persistence.Impl.Commands
{
    using System.Diagnostics.Contracts;

    using Ach.Fulfillment.Data.Common;

    internal class RelationalUpdateCommand<TEntity> : RelationalActionCommand<CommonUpdateActionData<TEntity>>
    {
        #region Public Methods and Operators

        public override void Execute(CommonUpdateActionData<TEntity> queryData)
        {
            Contract.Assert(queryData != null);
            this.Session.Update(queryData.Instance);
        }

        #endregion
    }
}