namespace Ach.Fulfillment.Persistence.Impl.Commands
{
    using System.Linq;

    using Ach.Fulfillment.Data.Common;

    internal class RelationalSpecificationCommand<TResult> : RelationalQueryCommand<ISpecification<TResult>, TResult>
    {
        public override IQueryable<TResult> Execute(ISpecification<TResult> queryData)
        {
            var result = this.Session
                .Apply(queryData);
            
            return result;
        }

        public override int RowCount(ISpecification<TResult> queryData)
        {
            var result = this.Session
                .Count(queryData);
            return result;
        }
    }
}