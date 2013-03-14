namespace Ach.Fulfillment.Business
{
    using System.IO;

    using Ach.Fulfillment.Data;

    using Renci.SshNet;

    public interface IRemoteAccessManager
    {
        // todo: add realization
        // todo: move credentials to database
        void Upload(PasswordConnectionInfo connectionInfo, string name, Stream stream);
        /*using (var sftp = new SftpClient(connectionInfo))
            {
                try
                {
                    sftp.Connect();

                    var fileName = achFile.Name + ".ach";
                    sftp.UploadFile(stream, fileName);
                }
                finally
                {
                    sftp.Disconnect();
                }
            }*/

        AchFileStatus GetStatus(string name);
    }
}