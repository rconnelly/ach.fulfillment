namespace Ach.Fulfillment.Data.Specifications.AchTransactions
{
    using Ach.Fulfillment.Data.Common;

    public class UpdateAchTransactionStatusByAchFileId : IActionData
    {
        public AchTransactionStatus Status { get; set; }

        public long AchFileId { get; set; }
    }
}