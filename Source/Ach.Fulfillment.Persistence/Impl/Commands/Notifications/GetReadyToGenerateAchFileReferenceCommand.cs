namespace Ach.Fulfillment.Persistence.Impl.Commands.Notifications
{
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications.Notifications;

    internal class GetReadyToGenerateAchFileReferenceCommand : GetRetriableReadyToOperationAchFileReferenceCommand<ReadyToGenerateAchFileReference>
    {
        public GetReadyToGenerateAchFileReferenceCommand()
            : base(AchFileStatus.Created)
        {
        }
    }
}