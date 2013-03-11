namespace Ach.Fulfillment.Data.Specifications.AchTransactions
{
    using Ach.Fulfillment.Data.Common;

    public class UnnotifiedAchTransactions : IQueryData<AchTransactionEntity>
    {
        public long AchFileId { get; set; }
    }
}