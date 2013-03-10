namespace Ach.Fulfillment.Data
{
    public enum AchTransactionStatus
    {
        Created = 0,
        Grouped = 1,
        Batched = 2,
        Sent = 3,
        Rejected = 4,
        Accepted = 5
    }
}