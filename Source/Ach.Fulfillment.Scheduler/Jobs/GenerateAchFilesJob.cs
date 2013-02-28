namespace Ach.Fulfillment.Scheduler.Jobs
{
    using System.Diagnostics.Contracts;

    using Business;

    using Microsoft.Practices.Unity;

    using Quartz;

    using Renci.SshNet;

    [DisallowConcurrentExecution]
    public class GenerateAchFilesJob : BaseJob
    {
        #region Public Properties

        [Dependency]
        public IAchFileManager Manager { get; set; }

        #endregion

        #region Public Methods and Operators

        protected override void ExecuteCore(IJobExecutionContext context)
        {
            var dataMap = context.JobDetail.JobDataMap;
            /*var achfilesStore = dataMap.GetString("AchFilesStore");*/
            var ftphost = dataMap.GetString("FtpHost");
            var userId = dataMap.GetString("UserId");
            var password = dataMap.GetString("Password");

            Contract.Assert(!string.IsNullOrEmpty(ftphost));
            Contract.Assert(!string.IsNullOrEmpty(userId));
            Contract.Assert(!string.IsNullOrEmpty(password));
            var connectionInfo = new PasswordConnectionInfo(ftphost, userId, password);
            
            this.Manager.Generate();
            this.Manager.Upload(connectionInfo);
        }

        #endregion
    }
}