﻿using System;
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
                var achFile = Manager.Generate();

                if (achFile != null & achFile.Length > 0)
                {
                    var dataMap = context.JobDetail.JobDataMap;
                    var achfilesStore = dataMap.GetString("AchFilesStore");
                    var newFileName = DateTime.Now.ToString("yyyyMMddHHmmss");
                    var newPath = System.IO.Path.Combine(achfilesStore, newFileName + ".txt");

                    var file = new System.IO.StreamWriter(newPath);
                    file.Write(achFile);
                    file.Flush();
                    file.Close();
                }
            }

        }
        
    }
}
