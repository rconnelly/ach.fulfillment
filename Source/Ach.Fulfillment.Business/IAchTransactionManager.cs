namespace Ach.Fulfillment.Business
{
    using System.Collections.Generic;
    using Data;

    public interface IAchTransactionManager : IManager<AchTransactionEntity>
    {
        void Generate(string achfilesStore);

        void ChangeAchTransactionStatus(List<AchTransactionEntity> transactions, AchTransactionStatus status);

        FileEntity CreateFileForPartnerTransactions(PartnerEntity partner, List<AchTransactionEntity> transactionEntities, string filename);
    }
}