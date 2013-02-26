namespace Ach.Fulfillment.Scheduler.Jobs
{
    using Ach.Fulfillment.Business;
    using Ach.Fulfillment.Scheduler.Common;

    using Microsoft.Practices.Unity;

    using Quartz;

    using global::Common.Logging;

    public class CleanUpJob : BaseJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CheckStatusFilesJob));

        #region Public Properties

        [Dependency]
        public IAchFileManager AchFileManager { get; set; }

        #endregion

        #region Public Methods and Operators

        protected override void InternalExecute(IJobExecutionContext context)
        {
            Logger.Info("CleanUpJob started...");

            this.AchFileManager.CleanUpCompletedFiles();

            Logger.Info("CleanUpJob finished...");
        }

        #endregion


    }
}
