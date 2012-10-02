using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Ach.Fulfillment.Common.Unity;
using Microsoft.Practices.Unity;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace Ach.Fulfillment.Scheduler.Configuration
{
    public class QuartzContainerExtension : UnityContainerExtension, IDisposable
    {
        private IScheduler scheduler;
        private ISchedulerFactory schedulerfactory;

        protected override void Initialize()
        {            
            Container.RegisterType<IJobFactory, UnityJobFactory>();
            Container.RegisterInstance<ISchedulerFactory>(new StdSchedulerFactory());
            schedulerfactory = Container.Resolve<ISchedulerFactory>();
            scheduler = schedulerfactory.GetScheduler();
            scheduler.JobFactory = Container.Resolve<IJobFactory>();
            scheduler.Start();
        }

        public void Dispose()
        {
            if (scheduler != null)
                scheduler.Shutdown();
        }
    }
}
