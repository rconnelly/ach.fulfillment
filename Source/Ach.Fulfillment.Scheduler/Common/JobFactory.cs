namespace Ach.Fulfillment.Scheduler.Common
{
    using System;
    using System.Diagnostics.Contracts;

    using Microsoft.Practices.ServiceLocation;

    using Quartz;
    using Quartz.Spi;

    internal class JobFactory : IJobFactory
    {
        #region Constants and Fields

        private readonly IServiceLocator serviceLocator;

        #endregion

        #region Constructors and Destructors

        public JobFactory(IServiceLocator serviceLocator)
        {
            this.serviceLocator = serviceLocator;
        }

        #endregion

        #region Public Methods and Operators

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            try
            {
                var job = this.serviceLocator.GetInstance(bundle.JobDetail.JobType) as IJob;
                Contract.Assert(job != null);
                return job;
            }
            catch (Exception exception)
            {
                throw new SchedulerException("Error on creation of new job", exception);
            }
        }

        #endregion
    }
}