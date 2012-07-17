namespace Ach.Fulfillment.Data.Specifications
{
    using System;
    using System.Linq.Expressions;

    public class RoleByName : SpecificationBase<RoleEntity>
    {
        private readonly string name;

        public RoleByName(string name)
        {
            this.name = name;
        }

        public override Expression<Func<RoleEntity, bool>> IsSatisfiedBy()
        {
            return m => m.Name == this.name;
        }
    }
}