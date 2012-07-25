namespace Ach.Fulfillment.Data.Common
{
    public interface IPagedSpecification<T> : ISpecification<T>
    {
        int PageIndex { get; }

        int PageSize { get; }
    }
}