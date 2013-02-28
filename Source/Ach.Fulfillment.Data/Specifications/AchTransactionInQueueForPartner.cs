namespace Ach.Fulfillment.Data.Specifications
{
    using System;
    using System.Linq.Expressions;

    using Ach.Fulfillment.Data.Common;

    public class AchTransactionInQueueForPartner : SpecificationBase<AchTransactionEntity>
    {
        private readonly PartnerEntity partner;

        public AchTransactionInQueueForPartner(PartnerEntity partner)
        {
            this.partner = partner;
        }

        public override Expression<Func<AchTransactionEntity, bool>> IsSatisfiedBy()
        {
            return m => m.Partner == this.partner && m.Status == AchTransactionStatus.Created && !m.Locked;
        }
    }
}   