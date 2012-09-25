using System;
using Ach.Fulfillment.Business;
using Common.Logging;
using Quartz;
using Microsoft.Practices.Unity;

namespace Ach.Fulfillment.Scheduler
{
   public class GenerateAchFilesJob:IJob
    {
       
       [Dependency]
        public IAchTransactionManager Manager { get; set; }

       //public GenerateAchFilesJob()
       //{
       //    Manager = new AchTransactionManager();
       //}

        public void Execute(IJobExecutionContext context)
        {
          
           var achFile = Manager.Generate();

            var dataMap = context.JobDetail.JobDataMap;

            var achfilesStore = dataMap.GetString("AchFilesStore");
            var newFileName = DateTime.UtcNow.ToString();
            var newPath = System.IO.Path.Combine(achfilesStore, "achfile.txt");

            var file = new System.IO.StreamWriter(newPath);
            file.Write("achfile");
            file.Flush();
            file.Close();

        }
    }
}
