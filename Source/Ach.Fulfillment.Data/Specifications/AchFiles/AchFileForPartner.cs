namespace Ach.Fulfillment.Data.Specifications.AchFiles
{
    using System;
    using System.Linq.Expressions;

    using Ach.Fulfillment.Data.Common;

    public class AchFileForPartner : SpecificationBase<AchFileEntity>
    {
        private readonly PartnerEntity partnerEntity;

        public AchFileForPartner(PartnerEntity partner)
        {
            this.partnerEntity = partner;
        }

        public override Expression<Func<AchFileEntity, bool>> IsSatisfiedBy()
        {
            return m => m.Partner == this.partnerEntity;
        }
    }
}