namespace Ach.Fulfillment.Business
{
    using System.Collections.Generic;

    using Data;

    public interface IAchTransactionManager
    {
        AchTransactionEntity Create(AchTransactionEntity instance);

        AchTransactionEntity Load(long id);

        void UpdateStatus(AchTransactionStatus status, IList<AchTransactionEntity> transactions);

        IEnumerable<AchTransactionEntity> GetEnqueued(PartnerEntity partner, bool toLock = true);

        void Lock(IList<AchTransactionEntity> transactions);

        void UnLock(IList<AchTransactionEntity> transactions);

        void SendAchTransactionNotification(IList<AchTransactionEntity> transactions);
    }
}