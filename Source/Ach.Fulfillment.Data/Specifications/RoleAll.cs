namespace Ach.Fulfillment.Data.Specifications
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    public class RoleAll : SpecificationBase<RoleEntity>, IOrderedSpecification<RoleEntity>
    {
        public override Expression<Func<RoleEntity, bool>> IsSatisfiedBy()
        {
            return _ => true;
        }

        public IQueryable<RoleEntity> Order(IQueryable<RoleEntity> query)
        {
            return query.OrderBy(m => m.Created);
        }
    }
}