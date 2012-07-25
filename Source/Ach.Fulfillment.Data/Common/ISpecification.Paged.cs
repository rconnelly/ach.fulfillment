namespace Ach.Fulfillment.Data.Common
{
    public interface ISpecificationPaged<T> : ISpecification<T>
    {
        int PageIndex { get; }

        int PageSize { get; }
    }
}