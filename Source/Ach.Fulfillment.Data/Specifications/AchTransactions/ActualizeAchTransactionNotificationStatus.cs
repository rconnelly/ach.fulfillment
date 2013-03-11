namespace Ach.Fulfillment.Data.Specifications.AchTransactions
{
    using Ach.Fulfillment.Data.Common;

    public class ActualizeAchTransactionNotificationStatus : IActionData
    {
        public long Id { get; set; }

        public AchTransactionStatus Status { get; set; }

        public AchTransactionStatus NotifiedStatus { get; set; }

        public bool Updated { get; set; }
    }
}