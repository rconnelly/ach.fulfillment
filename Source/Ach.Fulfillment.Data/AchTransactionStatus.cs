namespace Ach.Fulfillment.Data
{
    public enum AchTransactionStatus
    {
        None = 0,
        Created = 1,
        Grouped = 2,
        Batched = 3,
        Sent = 4,
        Rejected = 5,
        Accepted = 6
    }
}