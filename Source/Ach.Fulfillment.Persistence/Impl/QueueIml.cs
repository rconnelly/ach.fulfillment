namespace Ach.Fulfillment.Persistence.Impl
{
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Common;

    using Microsoft.Practices.Unity;

    internal class QueueIml : IQueue
    {
        #region Public Properties

        [Dependency]
        public IRepository Repository { get; set; }

        #endregion

        #region Public Methods and Operators

        public void Enqueue<T>(T item)
            where T : IEnqueueData
        {
            this.Repository.Execute(item);
        }

        public T Dequeue<T>(IQueueQueryData queryData)
        {
            var result = this.Repository.FindOne<T>(queryData);
            return result;
        }

        #endregion
    }
}