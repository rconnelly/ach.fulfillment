namespace Ach.Fulfillment.Data.Common
{
    public abstract class SpecificationPagedBase<T> : SpecificationBase<T>, ISpecificationPaged<T>
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
}