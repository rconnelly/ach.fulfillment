namespace Ach.Fulfillment.Persistence.Impl.Commands
{
    using System.Collections.Generic;
    using System.Linq;

    using Ach.Fulfillment.Data.Specifications;

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
            return this.Session.Query<TResult>().Where(specification.IsSatisfiedBy());
        }
    }
}