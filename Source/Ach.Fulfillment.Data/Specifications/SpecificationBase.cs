namespace Ach.Fulfillment.Data.Specifications
{
    using LinqSpecs;

    public abstract class SpecificationBase<T> : Specification<T>, ISpecification<T>
    {
    }
}