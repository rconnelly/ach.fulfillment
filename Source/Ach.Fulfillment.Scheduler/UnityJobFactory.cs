using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Common.Logging;
using Microsoft.Practices.Unity;
using Quartz;
using Quartz.Spi;
using Quartz.Util;

namespace Ach.Fulfillment.Scheduler
{
    public class UnityJobFactory : IJobFactory
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(UnityJobFactory));

         public UnityJobFactory(IUnityContainer container)
        {
            Container = container;
        }

        [Dependency]
        public IUnityContainer Container { get; set; }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            try
            {
                var job = Container.Resolve(bundle.JobDetail.JobType) as IJob;
                return job;
            }
            catch (Exception exception)
            {
                throw new SchedulerException("Error on creation of new job", exception);
            }
        }
    }
}
