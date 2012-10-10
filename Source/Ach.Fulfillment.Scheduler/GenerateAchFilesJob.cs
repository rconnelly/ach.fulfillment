using System;
using System.Globalization;
using Ach.Fulfillment.Business;
using Common.Logging;
using Quartz;
using Microsoft.Practices.Unity;
using Ach.Fulfillment.Common;

namespace Ach.Fulfillment.Scheduler
{
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
