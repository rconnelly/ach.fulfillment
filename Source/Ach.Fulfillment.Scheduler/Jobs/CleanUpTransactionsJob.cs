namespace Ach.Fulfillment.Scheduler.Jobs
{
    using System;

    using Ach.Fulfillment.Business;
    using Ach.Fulfillment.Common;

    using Microsoft.Practices.Unity;

    using Quartz;

    public class CleanUpTransactionsJob : IJob
    {
        #region Public Properties

        [Dependency]
        public IAchFileManager AchFileManager { get; set; }

        #endregion

        #region Public Methods and Operators

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                using (new UnitOfWork())
                {
                    this.AchFileManager.CleanUpCompletedFiles();
                }
            }
            catch (Exception ex)
            {
                throw new JobExecutionException(ex);
            }
        }

        #endregion 
    }
}
