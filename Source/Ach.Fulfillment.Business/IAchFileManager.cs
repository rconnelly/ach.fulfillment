namespace Ach.Fulfillment.Business
{
    using Ach.Fulfillment.Data;

    using Renci.SshNet;

    public interface IAchFileManager
    {
        void UpdateStatus(AchFileEntity file, AchFileStatus status);

        void ProcessReadyToBeGroupedAchTransactions();

        void ProcessReadyToBeGeneratedAchFile();

        void ProcessReadyToBeUploadedAchFile();

        void ProcessReadyToBeAcceptedAchFile();

        void Cleanup();
    }
}
