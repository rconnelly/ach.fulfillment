namespace Ach.Fulfillment.Persistence.Impl.Commands.Notifications
{
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications.Notifications;

    internal class GetReadyToGenerateAchFileReferenceCommand : GetSimpleContentFromQueueCommand<ReadyToGenerateAchFileReference, ReferenceEntity>
    {
        public GetReadyToGenerateAchFileReferenceCommand()
            : base("CreatedAchFileQueue")
        {
        }
    }
}