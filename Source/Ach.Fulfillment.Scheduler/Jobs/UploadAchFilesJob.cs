namespace Ach.Fulfillment.Scheduler.Jobs
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Ach.Fulfillment.Business;
    using Ach.Fulfillment.Common;
    using Ach.Fulfillment.Data;

    using log4net;

    using Microsoft.Practices.Unity;

    using Quartz;

    using Renci.SshNet;

    // todo (ng): no trigger/job definition associated with current class
    // todo (ng): it is similar/duplicate to GenerateAchFilesJob
    public class UploadAchFilesJob : IJob
    {
       private static readonly ILog Logger = LogManager.GetLogger(typeof(UploadAchFilesJob));

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
                        var achFilesToUpload = this.Manager.AchFilesToUpload();
                        this.Uploadfiles(connectionInfo, achFilesToUpload);
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

        private void Uploadfiles(PasswordConnectionInfo connectionInfo, IEnumerable<AchFileEntity> achFilesToUpload)
        {
            using (var sftp = new SftpClient(connectionInfo))
            {          
                try
                {
                    sftp.Connect();

                    foreach (var achfile in achFilesToUpload)
                    {
                        using (var stream = new MemoryStream())
                        {
                            var fileName = achfile.Name + ".ach";

                            var writer = new StreamWriter(stream);

                            /*writer.Write(achfile.AchFileBody);*/

                            writer.Flush();
                            stream.Position = 0;

                            sftp.UploadFile(stream, fileName);
                            this.Manager.UpdateStatus(achfile, AchFileStatus.Uploaded);
                            this.Manager.UnLock(achfile);
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
