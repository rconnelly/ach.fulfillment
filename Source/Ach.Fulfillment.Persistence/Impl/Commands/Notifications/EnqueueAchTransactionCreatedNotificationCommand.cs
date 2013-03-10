namespace Ach.Fulfillment.Persistence.Impl.Commands.Notifications
{
    using Ach.Fulfillment.Data.Specifications.Notifications;

    internal class EnqueueAchTransactionCreatedNotificationCommand : BaseEnqueueCommand<EnqueueAchTransactionCreatedNotification>
    {
        public EnqueueAchTransactionCreatedNotificationCommand()
            : base("FireAchTransactionCreated")
        {
        }
    }
}