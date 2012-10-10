using System.Collections.Generic;
using Ach.Fulfillment.Business.Impl;
using Ach.Fulfillment.Data;

namespace Ach.Fulfillment.Business
{
    public interface IAchTransactionManager : IManager<AchTransactionEntity>
    {
       // IFileManager FileManager { get; set; }

        void Generate();

        void Generate(string achfilesStore);

        void RemoveTransactionFromQueue(List<AchTransactionEntity> transactions);

        FileEntity CreateFileForPartnerTransactions(PartnerEntity partner, List<AchTransactionEntity> transactionEntities, string filename);

    }
}
