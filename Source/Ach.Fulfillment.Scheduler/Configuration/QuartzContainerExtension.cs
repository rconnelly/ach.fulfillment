using System;
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
