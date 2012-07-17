namespace Ach.Fulfillment.Data.Specifications
{
    using System;
    using System.Linq.Expressions;

    public class RoleUniqness : SpecificationInstanceBase<RoleEntity>
    {
        public override Expression<Func<RoleEntity, bool>> IsSatisfiedBy()
        {
            return m => m.Id != this.Instance.Id && m.Name == this.Instance.Name;
        }
    }
}