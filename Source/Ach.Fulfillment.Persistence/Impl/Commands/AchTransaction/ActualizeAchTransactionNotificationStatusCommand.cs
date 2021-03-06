namespace Ach.Fulfillment.Persistence.Impl.Commands.AchTransaction
{
    using System;
    using System.Diagnostics.Contracts;

    using Ach.Fulfillment.Data.Specifications.AchTransactions;

    internal class ActualizeAchTransactionNotificationStatusCommand :
        RelationalActionCommand<ActualizeAchTransactionNotificationStatus>
    {
        #region Constants

        private const string Sql = @"
update ach.AchTransaction set 
    NotifiedTransactionStatus = :notifiedstatus,
    Modified = :modified
where 
    AchTransactionId = :id and TransactionStatus = :status";

        #endregion

        #region Public Methods and Operators

        public override void Execute(ActualizeAchTransactionNotificationStatus actionData)
        {
            Contract.Assert(actionData != null);
            
            var query = this.Session.CreateSQLQuery(Sql);
            query.SetInt64("id", actionData.Id);
            query.SetInt32("notifiedstatus", (int)actionData.NotifiedStatus);
            query.SetInt32("status", (int)actionData.Status);
            query.SetDateTime("modified", DateTime.UtcNow);
            var affectedRows = query.ExecuteUpdate();
            actionData.Updated = affectedRows > 0;
        }

        #endregion
    }
}