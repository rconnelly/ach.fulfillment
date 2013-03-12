namespace Ach.Fulfillment.Business
{
    using Ach.Fulfillment.Data;

    using Renci.SshNet;

    public interface IAchFileManager
    {
        void ProcessReadyToBeGroupedAchTransactions();

        void ProcessReadyToBeGeneratedAchFile();

        void ProcessReadyToBeUploadedAchFile(PasswordConnectionInfo connectionInfo);

        void Cleanup();

        void ProcessReadyToBeAcceptedAchFile();

        void UpdateStatus(AchFileEntity file, AchFileStatus status);
    }
}
