namespace Ach.Fulfillment.Data.Specifications.AchTransactions
{
    using System;
    using System.Linq.Expressions;

    using Ach.Fulfillment.Data.Common;

    public class UngroupedAchTransactionByPartnerId : SpecificationBase<AchTransactionEntity>
    {
        public long PartnerId { get; set; }

        public override Expression<Func<AchTransactionEntity, bool>> IsSatisfiedBy()
        {
            var id = this.PartnerId;
            return t => t.AchFile == null && t.Partner.Id == id;
        }
    }
}