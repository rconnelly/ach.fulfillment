namespace Ach.Fulfillment.Business
{
    using Renci.SshNet;

    public interface IAchFileManager
    {
        void Generate();

        void Upload(PasswordConnectionInfo connectionInfo);

        void Cleanup();
    }
}
