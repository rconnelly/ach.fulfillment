namespace Ach.Fulfillment.Scheduler.Jobs
{
    using System;

    using Ach.Fulfillment.Business;
    using Ach.Fulfillment.Common;

    using Microsoft.Practices.Unity;

    using Quartz;

    using global::Common.Logging;

    public class SendWebhooksJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CheckStatusFilesJob));

        #region Public Properties

        [Dependency]
        public IWebhookManager WebhookManager { get; set; }

        [Dependency]
        public IClientNotifier ClientNotifier { get; set; }

        #endregion

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                using (new UnitOfWork())
                {
                    Logger.Info("SendWebhookJob started...");

                    WebhookManager.Send();

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
