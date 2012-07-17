namespace Ach.Fulfillment.Data.Common
{
    using System;
    using System.Linq.Expressions;

    public interface ISpecification<T> : IQueryData<T>
    {
        Expression<Func<T, bool>> IsSatisfiedBy();
    }
}