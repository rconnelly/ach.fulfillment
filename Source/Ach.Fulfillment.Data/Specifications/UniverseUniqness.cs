namespace Ach.Fulfillment.Data.Specifications
{
    using System;
    using System.Linq.Expressions;

    public class UniverseUniqness : SpecificationInstanceBase<UniverseEntity>
    {
        public override Expression<Func<UniverseEntity, bool>> IsSatisfiedBy()
        {
            return m => m.Id != this.Instance.Id && m.Login == this.Instance.Login;
        }
    }
}