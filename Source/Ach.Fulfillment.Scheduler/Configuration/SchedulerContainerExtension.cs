﻿namespace Ach.Fulfillment.Scheduler.Configuration
{
    using Ach.Fulfillment.Business;
    using Initialization.Configuration;
    using Common;

    using Microsoft.Practices.Unity;

    using Quartz;
    using Quartz.Spi;

    internal class SchedulerContainerExtension : InitializationContainerExtension
    {
        #region Methods

        protected override void Initialize()
        {
            this.Container.AddNewExtension<InitializationContainerExtension>();
            Container.RegisterType<ISchedulerFactory, SchedulerFactory>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IJobFactory, JobFactory>(new ContainerControlledLifetimeManager());             
        }

        #endregion
    }
}