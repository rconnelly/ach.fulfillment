namespace Ach.Fulfillment.Persistence.Impl.Commands
{
    using System.Diagnostics.Contracts;

    using Ach.Fulfillment.Data.Common;

    internal class RelationalCreateCommand<TEntity> : RelationalActionCommand<CommonCreateActionData<TEntity>>
    {
        #region Public Methods and Operators

        public override void Execute(CommonCreateActionData<TEntity> queryData)
        {
            Contract.Assert(queryData != null);
            this.Session.Save(queryData.Instance);
        }

        #endregion
    }
}