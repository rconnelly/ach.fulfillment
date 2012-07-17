namespace Ach.Fulfillment.Data.Specifications
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using Ach.Fulfillment.Data.Common;

    public class UniverseAll : SpecificationBase<UniverseEntity>, IOrderedSpecification<UniverseEntity>
    {
        private readonly bool withDeleted;

        public UniverseAll(bool withDeleted = false)
        {
            this.withDeleted = withDeleted;
        }

        public override Expression<Func<UniverseEntity, bool>> IsSatisfiedBy()
        {
            return m => this.withDeleted || !m.Deleted;
        }

        public IQueryable<UniverseEntity> Order(IQueryable<UniverseEntity> query)
        {
            return query.OrderBy(m => m.Created);
        }
    }
}
