namespace Ach.Fulfillment.Scheduler
{
    using Business;
    using Quartz;
    using Microsoft.Practices.Unity;
    using Common;

    public class GenerateAchFilesJob : IJob
    {
        [Dependency]
        public IAchTransactionManager Manager { get; set; }

        public void Execute(IJobExecutionContext context)
        {
            using (new UnitOfWork())
            {
                var dataMap = context.JobDetail.JobDataMap;
                var achfilesStore = dataMap.GetString("AchFilesStore");
                Manager.Generate(achfilesStore);

            }
       }

    }
}
