namespace Ach.Fulfillment.Data
{
    public enum AchTransactionStatus
    {
        Received = 0,
        Batched = 1,
        InProgress = 2,
        Completed = 3,
        Error = 4
    }
}