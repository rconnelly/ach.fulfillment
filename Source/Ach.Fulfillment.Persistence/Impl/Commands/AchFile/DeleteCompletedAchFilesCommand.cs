namespace Ach.Fulfillment.Persistence.Impl.Commands.AchFile
{
    using Ach.Fulfillment.Common.Transactions;
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications.AchFiles;

    internal class DeleteCompletedAchFilesCommand : RelationalActionCommand<DeleteCompletedAchFiles>
    {
        // todo: try to use linq
        private const string FindSql = @"
select AchFileId from ach.AchFile f
where 
	f.FileStatus in (:fileStatusAccepted,:fileStatusRejected)
	and not exists (
		select 1 from ach.AchTransaction t
        where 
		    t.AchFileId = f.AchFileId
			and 
			(
				t.NotifiedTransactionStatus != t.TransactionStatus
				or t.TransactionStatus not in (:transactionStatusAccepted,:transactionStatusRejected)
			)
	)
";

        // todo: try to use linq
        private const string DeleteTransactionsSql = @"
delete from [ach].[AchFile]
where AchFileId = :id";

        public override void Execute(DeleteCompletedAchFiles actionData)
        {
            var ids =
                this.Session.CreateSQLQuery(FindSql)
                    .SetInt32("fileStatusAccepted", (int)AchFileStatus.Accepted)
                    .SetInt32("fileStatusRejected", (int)AchFileStatus.Rejected)
                    .SetInt32("transactionStatusAccepted", (int)AchTransactionStatus.Accepted)
                    .SetInt32("transactionStatusRejected", (int)AchTransactionStatus.Rejected)
                    .List<int>();

            var query = this.Session.CreateSQLQuery(DeleteTransactionsSql);
            foreach (var id in ids)
            {
                using (var transaction = new Transaction())
                {
                    query
                        .SetInt64("id", id)
                        .ExecuteUpdate();

                    transaction.Complete();
                }    
            }
        }
    }
}