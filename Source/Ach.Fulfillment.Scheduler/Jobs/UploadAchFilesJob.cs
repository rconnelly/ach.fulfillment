namespace Ach.Fulfillment.Scheduler.Jobs
{
    using Ach.Fulfillment.Business;
    using Ach.Fulfillment.Scheduler.Common;
    
    using Microsoft.Practices.Unity;

    using Quartz;

    using global::Common.Logging;

    public class UploadAchFilesJob : BaseJob
    {
       private static readonly ILog Logger = LogManager.GetLogger(typeof(UploadAchFilesJob));

        #region Public Properties

        [Dependency]
        public IAchFileManager Manager { get; set; }

        #endregion

        #region Public Methods and Operators

        protected override void InternalExecute(IJobExecutionContext context)
        {
            Logger.Info("UploadAchFilesJob started...");

            var dataMap = context.JobDetail.JobDataMap;
            var ftphost = dataMap.GetString("FtpHost");
            var userId = dataMap.GetString("UserId");
            var password = dataMap.GetString("Password");

            if (!(string.IsNullOrEmpty(ftphost) && string.IsNullOrEmpty(userId) && string.IsNullOrEmpty(password)))
            {
                var achFilesToUpload = this.Manager.GetAchFilesDataForUploading();

                Manager.Uploadfiles(ftphost, userId, password, achFilesToUpload);
            }

            Logger.Info("UploadAchFilesJob finished...");
        }

        #endregion

    }
}
