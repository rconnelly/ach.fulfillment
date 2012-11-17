namespace Ach.Fulfillment.Business
{
    using System.Collections.Generic;

    using Ach.Fulfillment.Data;

    public interface IAchFileManager : IManager<AchFileEntity>
    {
        AchFileEntity Create(PartnerEntity partner, List<AchTransactionEntity> transactionEntities);

        void CleanUpCompletedFiles();

        void ChangeAchFilesStatus(AchFileEntity file, AchFileStatus status);

        void CreateFile(string achFile, string achfilesStore, string newFileName);

        List<AchFileEntity> AchFilesUpload();

        void UploadCompleted(AchFileEntity achFile);

        string GetNextIdModifier(PartnerEntity partner);

        void Generate(string achfilesStore);
    }
}
