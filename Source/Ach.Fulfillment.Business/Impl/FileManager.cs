namespace Ach.Fulfillment.Business.Impl
{
    using System.Collections.Generic;

    using Ach.Fulfillment.Common.Transactions;
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications;

    internal class FileManager : ManagerBase<FileEntity>, IFileManager
    {
        #region Public Methods

        public FileEntity CreateFileForPartnerTransactions(
            PartnerEntity partner, List<AchTransactionEntity> transactionEntities, string filename)
        {
            var fileEntity = new FileEntity
                                 {
                                     Name = filename,
                                     FileStatus = AchFileStatus.Created,
                                     Partner = partner,
                                     Transactions = transactionEntities,
                                     FileIdModifier = "A" // TODO calculate modifier
                                 };
            return this.Create(fileEntity);
        }

        public void CleanUpCompletedFiles()
        {
            var completedFiles = Repository.FindAll(new AchFileCompleted());

            using (var tx = new Transaction())
            {
                foreach (var completedFile in completedFiles)
                {
                    Repository.Delete(completedFile);
                }

                tx.Complete();
            }
        }

        public void ChangeFilesStatus(AchFileStatus status)
        {
            var fileToChange = Repository.Load<FileEntity>(0); //ToDo find real file
            using (var tx = new Transaction())
            {
                fileToChange.FileStatus = status;
                this.Update(fileToChange);
                tx.Complete();
            }
        }

        #endregion
    }
}
