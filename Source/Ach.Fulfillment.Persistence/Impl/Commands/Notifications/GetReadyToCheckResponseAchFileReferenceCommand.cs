namespace Ach.Fulfillment.Persistence.Impl.Commands.Notifications
{
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications.Notifications;

    internal class GetReadyToCheckResponseAchFileReferenceCommand : GetRetriableContentFromQueueCommand<ReadyToBeAcceptedAchFileReference, RetryReferenceEntity>
    {
        public GetReadyToCheckResponseAchFileReferenceCommand()
            : base("UploadedAchFileQueue")
        {
        }
    }
}