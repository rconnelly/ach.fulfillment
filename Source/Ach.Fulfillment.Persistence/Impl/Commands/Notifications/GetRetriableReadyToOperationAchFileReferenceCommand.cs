namespace Ach.Fulfillment.Persistence.Impl.Commands.Notifications
{
    using System.Globalization;

    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Common;

    internal class GetRetriableReadyToOperationAchFileReferenceCommand<TActionData> : GetRetriableContentFromQueueCommand<TActionData, RetryReferenceEntity>
        where TActionData : IQueryData<RetryReferenceEntity>
    {
        public GetRetriableReadyToOperationAchFileReferenceCommand(AchFileStatus status)
            : base(string.Format(CultureInfo.InvariantCulture, "{0}AchFileQueue", status))
        {
        }
    }
}