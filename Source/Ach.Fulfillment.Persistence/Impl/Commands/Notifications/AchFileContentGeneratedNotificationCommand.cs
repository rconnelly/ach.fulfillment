namespace Ach.Fulfillment.Persistence.Impl.Commands.Notifications
{
    using Ach.Fulfillment.Data.Specifications.Notifications;

    internal class AchFileContentGeneratedNotificationCommand : SendReferenceCommand<SendAchFileContentGeneratedNotification>
    {
        public AchFileContentGeneratedNotificationCommand()
            : base("AchFileReceiveService")
        {
        }
    }
}