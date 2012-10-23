namespace Ach.Fulfillment.Data.Specifications
{
    using System;
    using System.Linq.Expressions;
    using Common;

    public class CompletedAchTransaction : SpecificationBase<AchTransactionEntity>
    {
        public override Expression<Func<AchTransactionEntity, bool>> IsSatisfiedBy()
        {
            return m => m.TransactionStatus == AchTransactionStatus.Complete;
        }
    }
}