namespace Ach.Fulfillment.Data.Specifications
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using Ach.Fulfillment.Data.Common;

    public class UserPaged : SpecificationBase<UserEntity>, IPagedSpecification<UserEntity>, IOrderedSpecification<UserEntity>
    {
        private readonly bool withDeleted;

        public UserPaged(int pageIndex, int pageSize, bool withDeleted = false)
        {
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
            this.withDeleted = withDeleted;
        }

        public int PageIndex { get; private set; }

        public int PageSize { get; private set; }

        public override Expression<Func<UserEntity, bool>> IsSatisfiedBy()
        {
            return m => this.withDeleted || !m.Deleted;
        }

        public IQueryable<UserEntity> Order(IQueryable<UserEntity> query)
        {
            return query.OrderBy(m => m.Created);
        }
    }
}