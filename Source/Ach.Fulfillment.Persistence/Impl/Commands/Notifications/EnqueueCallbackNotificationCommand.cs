namespace Ach.Fulfillment.Persistence.Impl.Commands.Notifications
{
    using Ach.Fulfillment.Data.Specifications.Notifications;

    internal class EnqueueCallbackNotificationCommand : BaseEnqueueCommand<EnqueueCallbackNotification>
    {
        public EnqueueCallbackNotificationCommand()
            : base("EnqueueCallbackNotification")
        {
        }
    }
}