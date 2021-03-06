namespace Ach.Fulfillment.Persistence.Impl.Commands.AchTransaction
{
    using System;

    using Ach.Fulfillment.Data.Specifications.AchTransactions;

    internal class GroupAchTransactionToAchFileCommand : RelationalActionCommand<GroupAchTransactionToAchFile>
    {
        #region Constants

        public const string Sql = @"
update ach.AchTransaction 
set 
    AchFileId = :id,
    Modified = :modified
where 
    AchFileId is null";

        #endregion

        #region Public Methods and Operators

        public override void Execute(GroupAchTransactionToAchFile actionData)
        {
            var query = this.Session.CreateSQLQuery(Sql);
            query.SetInt64("id", actionData.AchFileId);
            query.SetDateTime("modified", DateTime.UtcNow);
            actionData.AffectedAchTransactionsCount = query.ExecuteUpdate();
        }

        #endregion
    }
}