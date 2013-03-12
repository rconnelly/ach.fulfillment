namespace Ach.Fulfillment.Persistence
{
    using System.Diagnostics.Contracts;

    using Ach.Fulfillment.Data.Common;

    public static class QueueExtension
    {
        public static T Dequeue<T>(this IQueue queue, IQueueQueryData<T> queryData)
        {
            Contract.Assert(queue != null);
            return queue.Dequeue<T>(queryData);
        }
    }
}