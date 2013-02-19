namespace Ach.Fulfillment.Data.Specifications
{
    using System;
    using System.Linq.Expressions;

    using Ach.Fulfillment.Data.Common;

    public class BatchedAchTransaction : SpecificationBase<AchTransactionEntity>
    {
        public override Expression<Func<AchTransactionEntity, bool>> IsSatisfiedBy()
        {
            // todo: does it work?
            return m => m.TransactionStatus == AchTransactionStatus.Batched;
        }
    }
}