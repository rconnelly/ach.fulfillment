namespace Ach.Fulfillment.Business.Impl.Strategies.Enumerators
{
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Common;
    using Ach.Fulfillment.Persistence;

    internal class ReadyForOperationAchFileEnumerator<TQuery> : ReadyForOperationAchFileEnumerator<TQuery, ReferenceEntity>
        where TQuery : IQueueQueryData<ReferenceEntity>, new() 
    {
        public ReadyForOperationAchFileEnumerator(IQueue queue, IRepository repository, string operation)
            : base(queue, repository, operation)
        {
        }
    }
}