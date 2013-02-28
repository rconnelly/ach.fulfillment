﻿namespace Ach.Fulfillment.Scheduler.Jobs
{
    using Ach.Fulfillment.Business;

    using Microsoft.Practices.Unity;

    using Quartz;

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
