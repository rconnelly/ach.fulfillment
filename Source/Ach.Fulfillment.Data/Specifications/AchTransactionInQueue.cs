namespace Ach.Fulfillment.Data.Specifications
{
    using System;
    using System.Linq.Expressions;
    using Common;

    public class AchTransactionInQueue : SpecificationBase<AchTransactionEntity>
    {
        public override Expression<Func<AchTransactionEntity, bool>> IsSatisfiedBy()
        {
            // todo: does it work?
            return m => m.TransactionStatus == AchTransactionStatus.Received && !m.Locked;
        }
    }
}
