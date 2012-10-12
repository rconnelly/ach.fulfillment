namespace Ach.Fulfillment.Scheduler.Configuration
{
    using System;
    using Microsoft.Practices.Unity;
    using Quartz;
    using Quartz.Impl;
    using Quartz.Spi;

    public class QuartzContainerExtension : UnityContainerExtension, IDisposable
    {
        private IScheduler _scheduler;
        private ISchedulerFactory _schedulerfactory;

        protected override void Initialize()
        {            
            Container.RegisterType<IJobFactory, UnityJobFactory>();
            Container.RegisterInstance<ISchedulerFactory>(new StdSchedulerFactory());
            _schedulerfactory = Container.Resolve<ISchedulerFactory>();
            _scheduler = _schedulerfactory.GetScheduler();
            _scheduler.JobFactory = Container.Resolve<IJobFactory>();
            _scheduler.Start();
        }

        public void Dispose()
        {
            if (_scheduler != null)
                _scheduler.Shutdown();
        }
    }
}
