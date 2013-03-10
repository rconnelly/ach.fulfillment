namespace Ach.Fulfillment.Persistence.Impl.Commands.Notifications
{
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications.Notifications;

    internal class GetReadyToBeGroupedAchTransactionReferenceCommand : GetSimpleContentFromQueueCommand<ReadyToBeGroupedAchTransactionReference, AchTransactionReferenceEntity>
    {
        public GetReadyToBeGroupedAchTransactionReferenceCommand()
            : base("CreatedAchTransactionQueue")
        {
        }
    }
}