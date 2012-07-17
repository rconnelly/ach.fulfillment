namespace Ach.Fulfillment.Data.Common
{
    using System.Linq;

    public interface IOrderedSpecification<T> : ISpecification<T>
    {
        IQueryable<T> Order(IQueryable<T> query);
    }
}