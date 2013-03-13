namespace Ach.Fulfillment.Business.Impl.Strategies.Enumerators
{
    using System.Diagnostics.Contracts;

    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications.Notifications;
    using Ach.Fulfillment.Persistence;

    internal class ReadyToBeGroupedAchTransactionReferenceEnumerator : BaseEnumerator<AchTransactionReferenceEntity>
    {
        #region Fields

        private readonly int limit;

        private readonly IQueue queue;

        private int index;

        #endregion

        #region Constructors and Destructors

        public ReadyToBeGroupedAchTransactionReferenceEnumerator(IQueue queue, int limit)
        {
            Contract.Assert(queue != null);
            this.queue = queue;
            this.limit = limit;
        }

        #endregion

        #region Public Methods and Operators

        public override bool MoveNext()
        {
            var fetched = false;
            AchTransactionReferenceEntity reference = null;
            if (this.index < this.limit)
            {
                reference = this.queue.Dequeue(new ReadyToBeGroupedAchTransactionReference());
                fetched = reference != null;
                this.index++;
            }

            this.Current = reference;
            return fetched;
        }

        public override void Reset()
        {
            this.index = 0;
        }

        #endregion
    }
}