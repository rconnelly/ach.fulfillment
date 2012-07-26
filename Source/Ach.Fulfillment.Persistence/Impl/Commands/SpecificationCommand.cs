namespace Ach.Fulfillment.Persistence.Impl.Commands
{
    using System.Linq;

    using Ach.Fulfillment.Data.Common;

    internal class SpecificationCommand<TResult> : CommandBase<ISpecification<TResult>, TResult>
    {
        public override IQueryable<TResult> Execute(ISpecification<TResult> queryData)
        {
            var query = this.Session
                .Page(queryData)
                .Where(queryData)
                .Order(queryData);
            return query;
        }

        public override int RowCount(ISpecification<TResult> queryData)
        {
            var result = this.Session
                .Count(queryData);
            return result;
        }
    }
}