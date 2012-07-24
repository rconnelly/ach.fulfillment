namespace Ach.Fulfillment.Persistence.Impl.Commands
{
    using System.Collections.Generic;
    using System.Linq;

    using Ach.Fulfillment.Data.Common;

    using NHibernate.Linq;

    internal class SpecificationCommand<TResult> : CommandBase<ISpecification<TResult>, TResult>
    {
        public override IEnumerable<TResult> FindAll(ISpecification<TResult> queryData)
        {
            var query = this.GetQuery(queryData);
            var orderedSpecification = queryData as IOrderedSpecification<TResult>;
            if (orderedSpecification != null)
            {
                query = orderedSpecification.Order(query);
            }

            return query.ToList();
        }

        public override TResult FindOne(ISpecification<TResult> queryData)
        {
            var query = this.GetQuery(queryData);
            var orderedSpecification = queryData as IOrderedSpecification<TResult>;
            if (orderedSpecification != null)
            {
                query = orderedSpecification.Order(query);
            }

            return query.FirstOrDefault();
        }

        public override int RowCount(ISpecification<TResult> queryData)
        {
            var query = this.GetQuery(queryData);
            return query.Count();
        }

        private IQueryable<TResult> GetQuery(ISpecification<TResult> specification)
        {
            IQueryable<TResult> result;

            // we may add more specifications and if is not the best approach - consider something better
            var pagedSpecification = specification as IPagedSpecification<TResult>;

            if (pagedSpecification != null)
            {
                result = this.Session.Query<TResult>()
                    .Skip(pagedSpecification.PageIndex * pagedSpecification.PageSize)
                    .Take(pagedSpecification.PageSize)
                    .Where(specification.IsSatisfiedBy());
            }
            else
            {
                result = this.Session.Query<TResult>().Where(specification.IsSatisfiedBy());
            }

            return result;
        }
    }
}