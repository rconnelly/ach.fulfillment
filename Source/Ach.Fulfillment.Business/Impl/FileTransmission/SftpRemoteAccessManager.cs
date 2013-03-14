namespace Ach.Fulfillment.Business.Impl.FileTransmission
{
    using System;
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
            Connect(
                sftp =>
                    {
                        var fileName = name + ".ach";
                        sftp.UploadFile(stream, fileName, true);
                    });
        }

        public AchFileStatus GetStatus(string name)
        {
            Contract.Assert(!string.IsNullOrEmpty(name));

            var status = AchFileStatus.None;
            Connect(
                sftp =>
                {
                    var fileName = name + ".ach.response";
                    if (sftp.Exists(fileName))
                    {
                        var content = sftp.ReadAllText(fileName);
                        if (string.Equals("A", content, StringComparison.InvariantCultureIgnoreCase))
                        {
                            status = AchFileStatus.Accepted;
                        }
                        else if (string.Equals("R", content, StringComparison.InvariantCultureIgnoreCase))
                        {
                            status = AchFileStatus.Rejected;
                        }
                        else
                        {
                            throw new InvalidDataException("Invalid file content: " + content);
                        }
                    }
                });

            return status;
        }

        private static void Connect(Action<SftpClient> action)
        {
            Contract.Assert(action != null);
            using (var connectionInfo = CreateConnectionInfo())
            {
                using (var sftp = new SftpClient(connectionInfo))
                {
                    try
                    {
                        sftp.Connect();
                        sftp.ChangeDirectory(Settings.Default.SFTPWorkingDirectory);

                        action(sftp);
                    }
                    finally
                    {
                        sftp.Disconnect();
                    }
                }
            }
        }

        private static PasswordConnectionInfo CreateConnectionInfo()
        {
            var result = new PasswordConnectionInfo(Settings.Default.SFTPHost, Settings.Default.SFTPLogin, Settings.Default.SFTPPassword);
            return result;
        }
    }
}