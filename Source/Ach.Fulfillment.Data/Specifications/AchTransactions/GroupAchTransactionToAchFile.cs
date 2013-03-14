namespace Ach.Fulfillment.Data.Specifications.AchTransactions
{
    using Ach.Fulfillment.Data.Common;

    public class GroupAchTransactionToAchFile : IActionData
    {
        public long AchFileId { get; set; }

        public int AffectedAchTransactionsCount { get; set; }  
    }
}