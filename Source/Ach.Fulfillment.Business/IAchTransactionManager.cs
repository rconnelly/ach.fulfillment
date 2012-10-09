using System.Collections.Generic;
using Ach.Fulfillment.Data;

namespace Ach.Fulfillment.Business
{
    public interface IAchTransactionManager : IManager<AchTransactionEntity>
    {
        void Generate();

        void RemoveTransactionFromQueue(List<AchTransactionEntity> transactions);

        void CreateFileForPartnerTransactions(PartnerEntity partner, List<AchTransactionEntity> transactionEntities);

    }
}
