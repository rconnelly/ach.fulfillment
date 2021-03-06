namespace Ach.Fulfillment.Data.Specifications.Partners
{
    using System;
    using System.Linq.Expressions;

    using Ach.Fulfillment.Data.Common;

    public class PartnerUniqness : SpecificationInstanceBase<PartnerEntity>
    {
        public override Expression<Func<PartnerEntity, bool>> IsSatisfiedBy()
        {
            return m => m.Id != this.Instance.Id && m.Name == this.Instance.Name;
        }
    }
}