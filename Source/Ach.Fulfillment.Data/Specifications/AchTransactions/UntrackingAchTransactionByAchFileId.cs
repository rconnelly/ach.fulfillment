namespace Ach.Fulfillment.Data.Specifications.AchTransactions
{
    using Ach.Fulfillment.Data.Common;

    public class UntrackingAchTransactionByAchFileId : IQueryData<AchTransactionEntity>
    {
        public long AchFileId { get; set; }
    }
}