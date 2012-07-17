namespace Ach.Fulfillment.Data.Specifications
{
    using System;
    using System.Linq.Expressions;

    using Ach.Fulfillment.Data.Common;

    public class CurrencyUniqness : SpecificationInstanceBase<CurrencyEntity>
    {
        public override Expression<Func<CurrencyEntity, bool>> IsSatisfiedBy()
        {
            return m => 
                m.Id != this.Instance.Id 
                && m.Universe.Id == this.Instance.Universe.Id 
                && m.Name == this.Instance.Name;
        }
    }
}