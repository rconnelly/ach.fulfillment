namespace Ach.Fulfillment.Scheduler.Common
{
    using System;

    using Ach.Fulfillment.Common;

    using global::Common.Logging;

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
                    this.ScheduleNextJob(context);
                }
            }
            catch (JobExecutionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                // todo (ng): use EHAB to log
                Logger.Error(ex);
                throw new JobExecutionException(ex);
            }
        }

        #endregion

        #region Methods

        protected abstract void ExecuteCore(IJobExecutionContext context);

        protected void ScheduleNextJob(IJobExecutionContext context)
        {
            var jobDataMap = context.MergedJobDataMap;
            var nextJob = jobDataMap.GetString("NEXT_JOB");

            // todo fire next job here
        }

        #endregion
    }
}
