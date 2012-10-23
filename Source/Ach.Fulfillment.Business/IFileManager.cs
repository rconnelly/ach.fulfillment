namespace Ach.Fulfillment.Business
{
    using System.Collections.Generic;

    using Ach.Fulfillment.Data;

    public interface IFileManager : IManager<FileEntity>
    {
        FileEntity CreateFileForPartnerTransactions(PartnerEntity partner, List<AchTransactionEntity> transactionEntities, string filename);

        void CleanUpCompletedFiles();

        void ChangeFilesStatus(AchFileStatus status);
    }
}
