namespace Ach.Fulfillment.Business.Impl.Strategies.Enumerators
{
    using Ach.Fulfillment.Data.Specifications.Notifications;
    using Ach.Fulfillment.Persistence;

    internal class ReadyToUploadAchFileEnumerator : ReadyForOperationAchFileEnumerator<ReadyToUploadAchFileReference>
    {
        public ReadyToUploadAchFileEnumerator(IQueue queue, IRepository repository)
            : base(queue, repository, "upload")
        {
        }
    }
}