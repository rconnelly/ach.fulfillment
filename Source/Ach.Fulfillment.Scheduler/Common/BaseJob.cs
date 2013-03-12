namespace Ach.Fulfillment.Scheduler.Common
{
    using System;

    using Ach.Fulfillment.Common;
    using Ach.Fulfillment.Scheduler.Configuration;

    using global::Common.Logging;

    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

    using Quartz;

    public abstract class BaseJob : IJob
    {
        #region Static Fields

        protected static readonly ILog Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Public Methods and Operators

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                using (new UnitOfWork())
                {
                    this.ExecuteCore(context);
                }
            }
            catch (Exception ex)
            {
                var rethrow = ExceptionPolicy.HandleException(ex, SchedulerContainerExtension.JobPolicy);
                if (rethrow)
                {
                    throw;
                }
            }
        }

        #endregion

        #region Methods

        protected abstract void ExecuteCore(IJobExecutionContext context);

        #endregion
    }
}
