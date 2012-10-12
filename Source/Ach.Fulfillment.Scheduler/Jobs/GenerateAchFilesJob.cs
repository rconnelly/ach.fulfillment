namespace Ach.Fulfillment.Scheduler.Jobs
{
    using System;
    using System.IO;

    using Ach.Fulfillment.Business;
    using Ach.Fulfillment.Common;

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
                    var achFileData = this.Manager.Generate();
                    if (!string.IsNullOrEmpty(achFileData))
                    {
                        var dataMap = context.JobDetail.JobDataMap;
                        var achfilesStore = dataMap.GetString("AchFilesStore");
                        var newFileName = DateTime.Now.ToString("yyyyMMddHHmmss");
                        var newPath = Path.Combine(achfilesStore, newFileName + ".txt");

                        using (var file = new StreamWriter(newPath))
                        {
                            file.Write(achFileData);
                        }
                    }
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