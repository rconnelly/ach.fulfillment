using System;
using System.Linq.Expressions;
using Ach.Fulfillment.Data.Common;

namespace Ach.Fulfillment.Data.Specifications
{
    public class AchTransactionInQueue: SpecificationBase<AchTransactionEntity>
    {
        public override Expression<Func<AchTransactionEntity, bool>> IsSatisfiedBy()
        {
            return m => m.TransactionStatus == AchTransactionStatus.Received;
        }
    }

    public class BatchedAchTransaction : SpecificationBase<AchTransactionEntity>
    {
        public override Expression<Func<AchTransactionEntity, bool>> IsSatisfiedBy()
        {
            return m => m.TransactionStatus == AchTransactionStatus.Batched;
        }
    }
}
