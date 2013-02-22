namespace Ach.Fulfillment.Scheduler.Common
{
    using System;
    using System.Diagnostics.Contracts;

    using Microsoft.Practices.Unity;

    using Quartz;
    using Quartz.Simpl;
    using Quartz.Spi;

    internal class JobFactory : SimpleJobFactory
    {
        #region Constants and Fields

        private readonly IUnityContainer container;

        #endregion

        #region Constructors and Destructors

        public JobFactory(IUnityContainer container)
        {
            this.container = container;
        }

        #endregion

        #region Public Methods and Operators

        public override IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            try
            {
                var job = base.NewJob(bundle, scheduler);
                Contract.Assert(job != null);
                this.container.BuildUp(job.GetType(), job);
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