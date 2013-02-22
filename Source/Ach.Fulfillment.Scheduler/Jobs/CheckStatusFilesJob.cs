namespace Ach.Fulfillment.Scheduler.Jobs
{
    using System;

    using Ach.Fulfillment.Business;
    using Ach.Fulfillment.Common;

    using global::Common.Logging;

    using Microsoft.Practices.Unity;

    using Quartz;

    public class CheckStatusFilesJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CheckStatusFilesJob));

        #region Public Properties

        [Dependency]
        public IAchFileManager AchFileManager { get; set; }

        [Dependency]
        public IAchTransactionManager AchTransactionManager { get; set; }

        #endregion

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                using (new UnitOfWork())
                {
                    Logger.Info("CheckStatusFilesJob started...");

                    // this.AchFileManager.ChangeAchFilesStatus();

                    // switch (status)
                    // {
                    // case AchFileStatus.Rejected: break;
                    // case AchFileStatus.Accepted: break;
                    // case AchFileStatus.Completed: break;
                    // case AchFileStatus.Uploaded: break;
                    // }
                    Logger.Info("CheckStatusFilesJob finished...");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new JobExecutionException(ex);
            }
        }
    }
}
