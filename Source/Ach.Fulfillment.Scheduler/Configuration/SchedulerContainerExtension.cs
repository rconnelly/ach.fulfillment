namespace Ach.Fulfillment.Scheduler.Configuration
{
    using Ach.Fulfillment.Initialization.Configuration;
    using Ach.Fulfillment.Scheduler.Common;

    using Microsoft.Practices.Unity;

    using Quartz;
    using Quartz.Spi;

    internal class SchedulerContainerExtension : InitializationContainerExtension
    {
        #region Methods

        protected override void Initialize()
        {
            this.Container.RegisterType<ISchedulerFactory, SchedulerFactory>(new ContainerControlledLifetimeManager());
            this.Container.RegisterType<IJobFactory, JobFactory>(new ContainerControlledLifetimeManager());
        }

        #endregion
    }
}