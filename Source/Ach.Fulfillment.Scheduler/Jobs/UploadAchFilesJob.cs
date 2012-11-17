namespace Ach.Fulfillment.Scheduler.Jobs
{
    using System;
    using System.IO;

    using Ach.Fulfillment.Business;
    using Ach.Fulfillment.Common;

    using Microsoft.Practices.Unity;

    using Quartz;

    using Renci.SshNet;

    public class UploadAchFilesJob : IJob
    {
       // private static readonly ILog Logger = LogManager.GetLogger(typeof(UploadAchFilesJob));

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
                        this.Uploadfiles(connectionInfo, achfilesStore);
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

        private void Uploadfiles(PasswordConnectionInfo connectionInfo, string sourceDirectory)
        {
            using (var sftp = new SftpClient(connectionInfo))
            {
                var achfiles = Manager.AchFilesUpload();
                              
                try
                {
                    sftp.Connect();

                    foreach (var achfile in achfiles)
                    {
                        var fileName = achfile.Name + ".ach";
                        var path = Path.Combine(sourceDirectory, fileName);

                        if (File.Exists(path))
                        {
                            using (var fileStream = File.OpenRead(path))
                            {
                                sftp.UploadFile(fileStream, fileName);
                                Manager.ChangeAchFilesStatus(achfile, Data.AchFileStatus.Uploaded);
                                Manager.UploadCompleted(achfile);
                            }
                        }
                        else
                        {
                            throw new FileNotFoundException("AchFile not found.");
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
