namespace Ach.Fulfillment.Persistence.Impl.Commands.AchTransaction
{
    using System;

    using Ach.Fulfillment.Data.Specifications.AchTransactions;

    internal class UpdateAchTransactionStatusByAchFileIdCommand :
        RelationalActionCommand<UpdateAchTransactionStatusByAchFileId>
    {
        #region Constants

        public const string Sql = @"
update ach.AchTransaction 
set 
    TransactionStatus = :status,
    Modified = :modified
where 
    AchFileId = :id";

        #endregion

        #region Public Methods and Operators

        public override void Execute(UpdateAchTransactionStatusByAchFileId actionData)
        {
            var query = this.Session.CreateSQLQuery(Sql);
            query.SetInt32("status", (int)actionData.Status);
            query.SetDateTime("modified", DateTime.UtcNow);
            query.SetInt64("id", actionData.AchFileId);
            query.ExecuteUpdate();
        }

        #endregion
    }
}