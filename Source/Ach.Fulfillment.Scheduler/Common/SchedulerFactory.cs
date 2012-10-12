namespace Ach.Fulfillment.Scheduler.Common
{
    using Microsoft.Practices.ServiceLocation;

    using Quartz;
    using Quartz.Impl;
    using Quartz.Spi;

    internal class SchedulerFactory : StdSchedulerFactory
    {
        #region Constants and Fields

        private readonly IServiceLocator serviceLocator;

        #endregion

        #region Constructors and Destructors

        public SchedulerFactory(IServiceLocator serviceLocator)
        {
            this.serviceLocator = serviceLocator;
        }

        #endregion

        #region Public Methods and Operators

        public override IScheduler GetScheduler()
        {
            var scheduler = base.GetScheduler();
            scheduler.JobFactory = this.serviceLocator.GetInstance<IJobFactory>();
            return scheduler;
        }

        #endregion
    }
}