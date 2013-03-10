namespace Ach.Fulfillment.Data.Specifications.Roles
{
    using System;
    using System.Linq.Expressions;

    using Ach.Fulfillment.Data.Common;

    public class RoleUniqness : SpecificationInstanceBase<RoleEntity>
    {
        public override Expression<Func<RoleEntity, bool>> IsSatisfiedBy()
        {
            return m => m.Id != this.Instance.Id && m.Name == this.Instance.Name;
        }
    }
}