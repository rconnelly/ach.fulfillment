namespace Ach.Fulfillment.Scheduler.Jobs
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Ach.Fulfillment.Business;
    using Ach.Fulfillment.Business.Impl;
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Scheduler.Common;

    using log4net;

    using Microsoft.Practices.Unity;

    using Quartz;

    using Renci.SshNet;

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
                var fileUploader = new FileUploader(ftphost, userId, password);
                var achFilesToUpload = this.Manager.GetAchFilesDataForUploading();
                
                fileUploader.Uploadfiles(achFilesToUpload);
            }

            Logger.Info("UploadAchFilesJob finished...");
        }

        #endregion

    }
}
