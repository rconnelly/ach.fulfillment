namespace Ach.Fulfillment.Business
{
    using System.Collections.Generic;

    using Ach.Fulfillment.Data;

    public interface IAchFileManager : IManager<AchFileEntity>
    {
        AchFileEntity Create(PartnerEntity partner, List<AchTransactionEntity> transactionEntities);

        void CleanUpCompletedFiles();

        void ChangeAchFilesStatus(AchFileEntity file, AchFileStatus status);

        List<AchFileEntity> AchFilesToUpload(bool lockRecords = true);

        void UnLock(AchFileEntity achFile);

        void Lock(AchFileEntity achFile);

        string GetNextIdModifier(PartnerEntity partner);

        void Generate();

        void GenerateForPartner(PartnerEntity partner);

        Dictionary<AchFileEntity, string> GetAchFilesDataForUploading();

        void Uploadfiles(string ftphost, string userId, string password, Dictionary<AchFileEntity, string> achFilesToUpload);
    }
}
