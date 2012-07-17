namespace Ach.Fulfillment.Data.Common
{
    using LinqSpecs;

    public abstract class SpecificationBase<T> : Specification<T>, ISpecification<T>
    {
    }
}