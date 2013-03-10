namespace Ach.Fulfillment.Business
{
    using Renci.SshNet;

    public interface IAchFileManager
    {
        void ProcessReadyToBeGroupedAchTransactions();

        bool ProcessReadyToBeGeneratedAchFile();

        bool ProcessReadyToBeUploadedAchFile(PasswordConnectionInfo connectionInfo);

        void Cleanup();

        bool ProcessReadyToBeAcceptedAchFile();
    }
}
