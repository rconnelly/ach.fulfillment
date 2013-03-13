namespace Ach.Fulfillment.Business.Impl.Strategies.Enumerators
{
    using Ach.Fulfillment.Data.Specifications.Notifications;
    using Ach.Fulfillment.Persistence;

    internal class ReadyToGenerateAchFileEnumerator : ReadyForOperationAchFileEnumerator<ReadyToGenerateAchFileReference>
    {
        public ReadyToGenerateAchFileEnumerator(IQueue queue, IRepository repository)
            : base(queue, repository, "generate")
        {
        }
    }
}