namespace Ach.Fulfillment.Scheduler.Jobs
{
    using System;

    using Ach.Fulfillment.Business;
    using Ach.Fulfillment.Common;

    using Microsoft.Practices.Unity;

    using Quartz;

    public class CheckStatusFilesJob : IJob
    {
        #region Public Properties

        [Dependency]
        public IFileManager FileManager { get; set; }

        [Dependency]
        public IAchTransactionManager AchTransactionManager { get; set; }

        #endregion

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                using (new UnitOfWork())
                {
                    // this.FileManager.ChangeFilesStatus();

                    // switch (status)
                    // {
                    // case AchFileStatus.Rejected: break;
                    // case AchFileStatus.Accepted: break;
                    // case AchFileStatus.Completed: break;
                    // case AchFileStatus.Uploaded: break;
                    // }
                }
            }
            catch (Exception ex)
            {
                throw new JobExecutionException(ex);
            }
        }
    }
}
