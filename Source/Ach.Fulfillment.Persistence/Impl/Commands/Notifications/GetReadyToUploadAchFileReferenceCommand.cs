namespace Ach.Fulfillment.Persistence.Impl.Commands.Notifications
{
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications.Notifications;

    internal class GetReadyToUploadAchFileReferenceCommand : GetSimpleContentFromQueueCommand<ReadyToUploadAchFileReference, ReferenceEntity>
    {
        public GetReadyToUploadAchFileReferenceCommand()
            : base("GeneratedAchFileQueue")
        {
        }
    }
}