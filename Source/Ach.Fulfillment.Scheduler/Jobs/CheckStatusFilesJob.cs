namespace Ach.Fulfillment.Scheduler.Jobs
{
    using Ach.Fulfillment.Business;
    using Ach.Fulfillment.Scheduler.Common;

    using global::Common.Logging;

    using Microsoft.Practices.Unity;

    using Quartz;

    public class CheckStatusFilesJob : BaseJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CheckStatusFilesJob));

        #region Public Properties

        [Dependency]
        public IAchFileManager AchFileManager { get; set; }

        [Dependency]
        public IAchTransactionManager AchTransactionManager { get; set; }

        #endregion

        protected override void InternalExecute(IJobExecutionContext context)
        {
            Logger.Info("CheckStatusFilesJob started...");

                    // todo (ng): Waiting for additional documentation
                    // todo (ng): Look at place where statusfiles are and get new 
                    // todo (ng): Parse status files
                    // todo (ng): Change statuses of Achfiles and included transactions to proper one
                    // todo (ng): Create webhook 

            Logger.Info("CheckStatusFilesJob finished...");
        }
    }
}
