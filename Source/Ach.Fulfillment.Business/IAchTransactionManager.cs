using System.Collections.Generic;
using Ach.Fulfillment.Data;

namespace Ach.Fulfillment.Business
{
    public interface IAchTransactionManager : IManager<AchTransactionEntity>
    {
        string Generate();
        void RemoveFromQueue(List<AchTransactionEntity> transactions);
    }
}
