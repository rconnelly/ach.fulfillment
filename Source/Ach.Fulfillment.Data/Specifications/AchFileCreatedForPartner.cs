namespace Ach.Fulfillment.Data.Specifications
{
    using System;
    using System.Linq.Expressions;

    using Ach.Fulfillment.Data.Common;

    public class AchFileCreatedForPartner : SpecificationBase<AchFileEntity>
    {
        private readonly PartnerEntity partnerEntity;

        public AchFileCreatedForPartner(PartnerEntity partner)
        {
            this.partnerEntity = partner;
        }

        public override Expression<Func<AchFileEntity, bool>> IsSatisfiedBy()
        {
            // todo: does it work?
            return m => m.FileStatus == AchFileStatus.Created && 
                m.Locked == false && 
                m.Partner == this.partnerEntity;
        }
    }
}