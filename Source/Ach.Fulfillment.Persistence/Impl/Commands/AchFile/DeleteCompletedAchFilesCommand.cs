namespace Ach.Fulfillment.Persistence.Impl.Commands.AchFile
{
    using System.Linq;

    using Ach.Fulfillment.Common.Transactions;
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications.AchFiles;

    using NHibernate.Linq;

    internal class DeleteCompletedAchFilesCommand : RelationalActionCommand<DeleteCompletedAchFiles>
    {
        public override void Execute(DeleteCompletedAchFiles actionData)
        {
            var ids = from f in this.Session.Query<AchFileEntity>() 
                     where 
                        (f.FileStatus == AchFileStatus.Accepted
                        || f.FileStatus == AchFileStatus.Rejected)
                        && f.Transactions.All(
                            t => t.NotifiedStatus == t.Status 
                                && (
                                    t.Status == AchTransactionStatus.Accepted 
                                    || t.Status == AchTransactionStatus.Rejected))
                     select f.Id;

            foreach (var id in ids)
            {
                using (var transaction = new Transaction())
                {
                    var file = this.Session.Load<AchFileEntity>(id);
                    this.Session.Delete(file);
                    transaction.Complete();
                }    
            }
        }
    }
}