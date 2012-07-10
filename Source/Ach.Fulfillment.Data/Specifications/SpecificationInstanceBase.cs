namespace Ach.Fulfillment.Data.Specifications
{
    using Ach.Fulfillment.Data.QueryData;

    public abstract class SpecificationInstanceBase<T> : SpecificationBase<T>, IInstanceQueryData<T>
    {
        public T Instance { get; set; }
    }
}