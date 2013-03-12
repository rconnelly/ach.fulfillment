namespace Ach.Fulfillment.Persistence
{
    using Ach.Fulfillment.Data.Common;

    public interface IQueue
    {
        void Enqueue<T>(T item) where T : IEnqueueData;

        T Dequeue<T>(IQueueQueryData queryData);
    }
}