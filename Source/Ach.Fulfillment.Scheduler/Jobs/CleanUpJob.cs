namespace Ach.Fulfillment.Scheduler.Jobs
{
    using Ach.Fulfillment.Business;
    using Ach.Fulfillment.Scheduler.Common;

    using Microsoft.Practices.Unity;

    using Quartz;

    [DisallowConcurrentExecution]
    public class CleanUpTransactionsJob : BaseJob
    {
        #region Public Properties

        [Dependency]
        public IAchFileManager Manager { get; set; }

        #endregion

        #region Public Methods and Operators

        protected override void ExecuteCore(IJobExecutionContext context)
        {
            this.Manager.Cleanup();
        }

        #endregion 
    }
}
