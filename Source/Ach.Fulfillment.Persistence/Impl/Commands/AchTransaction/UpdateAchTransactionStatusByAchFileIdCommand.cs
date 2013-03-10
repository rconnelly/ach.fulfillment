namespace Ach.Fulfillment.Persistence.Impl.Commands.AchTransaction
{
    using Ach.Fulfillment.Data.Specifications.AchTransactions;

    internal class UpdateAchTransactionStatusByAchFileIdCommand :
        RelationalActionCommand<UpdateAchTransactionStatusByAchFileId>
    {
        #region Constants

        public const string Sql = @"
update t
	set TransactionStatus = :Status
from  ach.AchTransaction t
	inner join ach.AchFileTransaction f
		on f.AchFileId = :AchFileId	and t.AchTransactionId = f.AchTransactionId";

        #endregion

        #region Public Methods and Operators

        public override void Execute(UpdateAchTransactionStatusByAchFileId actionData)
        {
            var query = this.Session.CreateSQLQuery(Sql);
            query.SetInt32("Status", (int)actionData.Status);
            query.SetInt64("AchFileId", actionData.AchFileId);
            query.ExecuteUpdate();
        }

        #endregion
    }
}