namespace Ach.Fulfillment.Scheduler.Common
{
    using Quartz;

    using global::Common.Logging;

    public class JobListener : IJobListener
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(JobListener));

        #region Public Properties

        public string Name { get; private set; }

        #endregion

        #region Public Methods

        public void JobToBeExecuted(IJobExecutionContext context)
        {
            throw new System.NotImplementedException();
        }

        public void JobExecutionVetoed(IJobExecutionContext context)
        {
            throw new System.NotImplementedException();
        }

        public void JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException)
        {
            var nextJob = JobBuilder.Create<BaseJob>().WithIdentity("job2").Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("job2Trigger")
                .StartNow()
                .Build();

            try
            {
                // schedule the job to run!
                context.Scheduler.ScheduleJob(nextJob, trigger);
            }
            catch (SchedulerException e)
            {
                log.Warn("Unable to schedule nextJob!");
            }
        }

        #endregion
    }
}
