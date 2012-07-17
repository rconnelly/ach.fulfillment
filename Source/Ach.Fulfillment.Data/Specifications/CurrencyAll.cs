namespace Ach.Fulfillment.Data.Specifications
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq.Expressions;

    using Ach.Fulfillment.Data.Common;

    public class CurrencyAll : SpecificationBase<CurrencyEntity>
    {
        private readonly UniverseEntity universe;

        public CurrencyAll(UniverseEntity universe)
        {
            Contract.Assert(universe != null);
            this.universe = universe;
        }

        public override Expression<Func<CurrencyEntity, bool>> IsSatisfiedBy()
        {
            return m =>
                   m.Universe.Id == this.universe.Id;
        }
    }
}