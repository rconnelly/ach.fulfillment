namespace Ach.Fulfillment.Scheduler.Common
{
    using System;

    using Ach.Fulfillment.Common;

    using Quartz;

    public abstract class BaseJob : IJob
    {
        // todo (ng): create EHAB policy exception handling. logging should be configured in EHAB
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                using (new UnitOfWork())
                {
                    InternalExecute(context);
                    ScheduleNextJob(context);
                }
            }
            catch (Exception ex)
            {
                throw new JobExecutionException(ex);
            }
        }

        protected abstract void InternalExecute(IJobExecutionContext context);

        protected void ScheduleNextJob(IJobExecutionContext context)
        {
            var jobDataMap = context.MergedJobDataMap;
            var nextJob = jobDataMap.GetString("NEXT_JOB"); 

            // todo fire next job here
        }

    }
}
