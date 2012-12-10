namespace Ach.Fulfillment.Business
{
    using System.Collections.Generic;
    using Data;

    public interface IAchTransactionManager : IManager<AchTransactionEntity>
    {
        void ChangeAchTransactionStatus(List<AchTransactionEntity> transactions, AchTransactionStatus status);

        void SendAchTransactionNotification(List<AchTransactionEntity> transactions);

        List<AchTransactionEntity> GetAllInQueue(bool toLock = true);

        List<AchTransactionEntity> GetAllInQueueForPartner(PartnerEntity partner, bool toLock = true);

        void UnLock(List<AchTransactionEntity> transactions);
    }
}