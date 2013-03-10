namespace Ach.Fulfillment.Persistence.Impl.Commands.Notifications
{
    using System.Globalization;

    using Ach.Fulfillment.Data.Specifications.Notifications;

    internal class EnqueueAchFileStatusChangedNotificationCommand : BaseEnqueueCommand<EnqueueAchFileStatusChangedNotification>
    {
        public override void Execute(EnqueueAchFileStatusChangedNotification actionData)
        {
            var destinationService = string.Format(CultureInfo.InvariantCulture, "FireAchFile{0}", actionData.FileStatus);
            this.Execute(actionData, destinationService);
        }
    }
}