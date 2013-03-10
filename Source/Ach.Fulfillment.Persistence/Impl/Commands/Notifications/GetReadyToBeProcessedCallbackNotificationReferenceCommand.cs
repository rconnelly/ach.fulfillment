namespace Ach.Fulfillment.Persistence.Impl.Commands.Notifications
{
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications.Notifications;

    internal class GetReadyToBeProcessedCallbackNotificationReferenceCommand : GetRetriableContentFromQueueCommand<ReadyToBeProcessedCallbackNotificationReference, RetryReferenceEntity>
    {
        public GetReadyToBeProcessedCallbackNotificationReferenceCommand()
            : base("CallbackNotificationQueue")
        {
        }
    }
}