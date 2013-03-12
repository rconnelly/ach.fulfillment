namespace Ach.Fulfillment.Scheduler.Jobs
{
    using System;

    using Ach.Fulfillment.Business;
    using Ach.Fulfillment.Common;

    using global::Common.Logging;

    using Microsoft.Practices.Unity;

    using Quartz;

    public class CallbackNotificationJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CheckStatusFilesJob));

        #region Public Properties

        [Dependency]
        public ICallbackNotificationManager CallbackNotificationManager { get; set; }

        #endregion

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                using (new UnitOfWork())
                {
                    Logger.Info("SendWebhookJob started...");

                    this.CallbackNotificationManager.DeliverRemoteNotifications();

                    Logger.Info("SendWebhookJob finished...");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new JobExecutionException(ex);
            }
        }
    }
}
