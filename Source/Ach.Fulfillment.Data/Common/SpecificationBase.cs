namespace Ach.Fulfillment.Data.Common
{
    using System.Linq;

    using LinqSpecs;

    public abstract class SpecificationBase<TEntity> : Specification<TEntity>, ISpecification<TEntity>
    {
        #region Public Properties

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        #endregion

        #region Public Methods and Operators

        public virtual IQueryable<TEntity> Order(IQueryable<TEntity> query)
        {
            return query;
        }

        #endregion
    }
}