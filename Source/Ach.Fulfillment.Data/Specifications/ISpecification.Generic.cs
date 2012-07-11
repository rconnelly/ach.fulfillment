namespace Ach.Fulfillment.Data.Specifications
{
    using System;
    using System.Linq.Expressions;

    using Ach.Fulfillment.Data.QueryData;

    public interface ISpecification<T> : IQueryData<T>
    {
        Expression<Func<T, bool>> IsSatisfiedBy();
    }
}