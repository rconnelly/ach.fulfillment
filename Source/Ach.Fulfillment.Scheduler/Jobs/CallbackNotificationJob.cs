namespace Ach.Fulfillment.Scheduler.Jobs
{
    using Ach.Fulfillment.Business;
    using Ach.Fulfillment.Scheduler.Common;

    using Microsoft.Practices.Unity;

    using Quartz;

    public class CallbackNotificationJob : BaseJob
    {
        #region Public Properties

        [Dependency]
        public ICallbackNotificationManager Manager { get; set; }

        #endregion

        #region Methods

        protected override void ExecuteCore(IJobExecutionContext context)
        {
            this.Manager.DeliverRemoteNotifications();
        }

        #endregion
    }
}