namespace Ach.Fulfillment.Persistence.Impl.Commands.Notifications
{
    using Ach.Fulfillment.Data.Specifications.Notifications;

    internal class AchTransactionGroupCreatedNotificationCommand : SendReferenceCommand<SendAchTransactionGroupCreatedNotification>
    {
        public AchTransactionGroupCreatedNotificationCommand()
            : base("AchTransactionGroupReceiveService")
        {
        }
    }
}