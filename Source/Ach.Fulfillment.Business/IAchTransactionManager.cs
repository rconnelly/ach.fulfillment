namespace Ach.Fulfillment.Business
{
    using System.Collections.Generic;
    using Data;

    public interface IAchTransactionManager : IManager<AchTransactionEntity>
    {
        void Generate();

        void Generate(string achfilesStore);

        void RemoveTransactionFromQueue(List<AchTransactionEntity> transactions);

        FileEntity CreateFileForPartnerTransactions(PartnerEntity partner, List<AchTransactionEntity> transactionEntities, string filename);

    }
}
