namespace Ach.Fulfillment.Scheduler.Jobs
{
    using System;
    using System.IO;

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
        public IAchTransactionManager Manager { get; set; }

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
                    this.Manager.Generate(achfilesStore);

                    var ftphost = dataMap.GetString("FtpHost");
                    var userId = dataMap.GetString("UserId");
                    var password = dataMap.GetString("Password");

                    if (!(string.IsNullOrEmpty(ftphost) 
                        && string.IsNullOrEmpty(userId) && string.IsNullOrEmpty(password)))
                    {
                        var connectionInfo = new PasswordConnectionInfo(ftphost, userId, password)
                            {
                                Timeout = TimeSpan.FromSeconds(60)
                            }; 
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
                var directory = new DirectoryInfo(sourceDirectory);
                var files = directory.GetFiles("*.ach");
                try
                {
                    sftp.Connect();

                    foreach (var file in files)
                    {
                        using (var fileStream = File.OpenRead(file.FullName))
                        {
                            sftp.UploadFile(fileStream, file.Name);
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