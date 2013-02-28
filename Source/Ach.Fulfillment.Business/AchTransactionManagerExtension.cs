namespace Ach.Fulfillment.Business
{
    using System.Diagnostics.Contracts;

    using Ach.Fulfillment.Data;

    public static class AchTransactionManagerExtension
    {
        public static void UpdateStatus(
            this IAchTransactionManager manager, AchTransactionStatus status, params AchTransactionEntity[] transactions)
        {
            Contract.Assert(manager != null);
            manager.UpdateStatus(status, transactions);
        }

        public static void SendAchTransactionNotification(
            this IAchTransactionManager manager, params AchTransactionEntity[] transactions)
        {
            Contract.Assert(manager != null);
            manager.SendAchTransactionNotification(transactions);
        }
    }
}