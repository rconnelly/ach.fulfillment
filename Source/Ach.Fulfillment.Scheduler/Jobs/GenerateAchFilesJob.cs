namespace Ach.Fulfillment.Scheduler.Jobs
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Ach.Fulfillment.Data;

    using Business;
    using Fulfillment.Common;

    using Microsoft.Practices.Unity;

    using Quartz;

    using Renci.SshNet;

    [DisallowConcurrentExecution]
    public class GenerateAchFilesJob : IJob
    {
        #region Public Properties

        [Dependency]
        public IAchFileManager Manager { get; set; }

        #endregion

        #region Public Methods and Operators

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                using (new UnitOfWork())
                {
                    var dataMap = context.JobDetail.JobDataMap;
                    var achfilesStore = dataMap.GetString("AchFilesStore");

                    var achFiles = this.Manager.Generate();

                    foreach (var newPath in achFiles.Select(achFile => Path.Combine(achfilesStore, achFile.Key.Name + ".ach")))
                    {
                        using (var file = new StreamWriter(newPath))
                        {
                            file.Write(newPath);
                            file.Flush();
                            file.Close();
                        }
                    }

                    var ftphost = dataMap.GetString("FtpHost");
                    var userId = dataMap.GetString("UserId");
                    var password = dataMap.GetString("Password");

                    if (!(string.IsNullOrEmpty(ftphost)
                        && string.IsNullOrEmpty(userId) && string.IsNullOrEmpty(password)))
                    {
                        var connectionInfo = new PasswordConnectionInfo(ftphost, userId, password);

                        // {
                            // Timeout = TimeSpan.FromSeconds(60)
                       // };
                        this.Uploadfiles(connectionInfo, achFiles);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new JobExecutionException(ex);
            }
        }

        #endregion

       #region Private Methods

        private void Uploadfiles(PasswordConnectionInfo connectionInfo, Dictionary<AchFileEntity, string> achFilesToUpload)
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
                                this.Manager.Lock(achfile.Key);

                                var writer = new StreamWriter(stream);
                                writer.Write(achfile.Value);
                                writer.Flush();
                                stream.Position = 0;
                                sftp.UploadFile(stream, fileName);
                                this.Manager.ChangeAchFilesStatus(achfile.Key, AchFileStatus.Uploaded);
                            }
                        }
                        finally
                        {
                            this.Manager.UnLock(achfile.Key);  
                        }
                    }
                }
                finally
                {
                    sftp.Disconnect();
                }
            }
        }

        #endregion
    }
}