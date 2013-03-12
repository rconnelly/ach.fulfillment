namespace Ach.Fulfillment.Business.Impl
{
    using Ach.Fulfillment.Business.Impl.Strategies.Processors;
    using Ach.Fulfillment.Persistence;

    using Microsoft.Practices.Unity;

    internal class CallbackNotificationManager : ICallbackNotificationManager
    {
        #region Public Properties

        [Dependency]
        public IRepository Repository { get; set; }

        [Dependency]
        public IQueue Queue { get; set; }

        #endregion

        #region Public Methods and Operators

        public void DeliverRemoteNotifications()
        {
            var processor = new CallbackNotificationProcessor(this.Queue, this.Repository);
            processor.Execute();
        }

        #endregion
    }
}