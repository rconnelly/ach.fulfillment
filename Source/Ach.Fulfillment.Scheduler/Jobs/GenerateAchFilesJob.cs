namespace Ach.Fulfillment.Scheduler.Jobs
{
    using System;
    using Business;
    using Fulfillment.Common;

    using Microsoft.Practices.Unity;

    using Quartz;

    [DisallowConcurrentExecution]
    public class GenerateAchFilesJob : IJob
    {
        #region Public Properties

        [Dependency]
        public IAchTransactionManager Manager { get; set; }

        #endregion

        #region Public Methods and Operators

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                using (new UnitOfWork())
                {
                    var dataMap = context.JobDetail.JobDataMap;
                    var achfilesStore = dataMap.GetString("AchFilesStore");
                    Manager.Generate(achfilesStore);                
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