namespace Ach.Fulfillment.Business.Impl
{
    using System.Collections.Generic;
    using System.IO;

    using Ach.Fulfillment.Data;
    using Renci.SshNet;

    public class FileUploader : IFileUploader
    {
        private readonly PasswordConnectionInfo connectionInfo;

        public FileUploader(string ftphost, string userId, string password)
        {
            this.connectionInfo = new PasswordConnectionInfo(ftphost, userId, password);
        }

        public void Uploadfiles(Dictionary<AchFileEntity, string> achFilesToUpload)
        {
            using (var sftp = new SftpClient(connectionInfo))
            {
                try
                {
                    sftp.Connect();

                    foreach (var achfile in achFilesToUpload)
                    {
                        try
                        {
                            using (var stream = new MemoryStream())
                            {
                                var fileName = achfile.Key.Name + ".ach";
                               // todo this.Manager.Lock(achfile.Key);

                                var writer = new StreamWriter(stream);
                                writer.Write(achfile.Value);
                                writer.Flush();
                                stream.Position = 0;
                                sftp.UploadFile(stream, fileName);
                                //todo this.Manager.ChangeAchFilesStatus(achfile.Key, AchFileStatus.Uploaded);
                            }
                        }
                        finally
                        {
                            // todo this.Manager.UnLock(achfile.Key);
                        }
                    }
                }
                finally
                {
                    sftp.Disconnect();
                }
            }
        }

    }
}
