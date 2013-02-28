namespace Ach.Fulfillment.Scheduler.Jobs
{
    using Ach.Fulfillment.Business;

    using Microsoft.Practices.Unity;

    using Quartz;

    public class CheckStatusFilesJob : BaseJob
    {
        #region Public Properties

        [Dependency]
        public IAchFileManager AchFileManager { get; set; }

        [Dependency]
        public IAchTransactionManager AchTransactionManager { get; set; }

        #endregion

        #region Methods

        protected override void ExecuteCore(IJobExecutionContext context)
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

        #endregion
    }
}