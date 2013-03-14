namespace Ach.Fulfillment.Business.Impl.FileTransmission
{
    using System.Diagnostics.Contracts;
    using System.IO;

    using Ach.Fulfillment.Business.Properties;
    using Ach.Fulfillment.Data;

    using Renci.SshNet;

    internal class SftpRemoteAccessManager : IRemoteAccessManager
    {
        public void Upload(string name, Stream stream)
        {
            Contract.Assert(!string.IsNullOrEmpty(name));
            Contract.Assert(stream != null);
            using (var connectionInfo = new PasswordConnectionInfo(Settings.Default.SFTPHost, Settings.Default.SFTPLogin, Settings.Default.SFTPPassword))
            {
                using (var sftp = new SftpClient(connectionInfo))
                {
                    try
                    {
                        sftp.Connect();
                        sftp.ChangeDirectory(Settings.Default.SFTPWorkingDirectory);
                        var fileName = name + ".ach";
                        sftp.UploadFile(stream, fileName);
                    }
                    finally
                    {
                        sftp.Disconnect();
                    }
                }
            }
        }

        public AchFileStatus GetStatus(string name)
        {
            throw new System.NotImplementedException();
        }
    }
}