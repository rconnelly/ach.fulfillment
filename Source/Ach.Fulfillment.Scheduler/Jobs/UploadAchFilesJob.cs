namespace Ach.Fulfillment.Scheduler.Jobs
{
    using System.Diagnostics.Contracts;

    using Ach.Fulfillment.Business;
    using Ach.Fulfillment.Scheduler.Common;
    
    using Microsoft.Practices.Unity;

    using Quartz;

    using Renci.SshNet;

    public class UploadAchFilesJob : BaseJob
    {
        #region Public Properties

        [Dependency]
        public IAchFileManager Manager { get; set; }

        #endregion

        #region Public Methods and Operators

        protected override void ExecuteCore(IJobExecutionContext context)
        {
            Logger.Info("UploadAchFilesJob started...");

            var dataMap = context.JobDetail.JobDataMap;
            var ftphost = dataMap.GetString("FtpHost");
            var userId = dataMap.GetString("UserId");
            var password = dataMap.GetString("Password");

            Contract.Assert(!string.IsNullOrEmpty(ftphost));
            Contract.Assert(!string.IsNullOrEmpty(userId));
            Contract.Assert(!string.IsNullOrEmpty(password));
            var connectionInfo = new PasswordConnectionInfo(ftphost, userId, password);

            this.Manager.ProcessReadyToBeUploadedAchFile(connectionInfo);

            Logger.Info("UploadAchFilesJob finished...");
        }

        #endregion
    }
}
