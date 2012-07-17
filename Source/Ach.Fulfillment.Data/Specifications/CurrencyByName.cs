namespace Ach.Fulfillment.Data.Specifications
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq.Expressions;

    using Ach.Fulfillment.Data.Common;

    public class CurrencyByName : SpecificationBase<CurrencyEntity>
    {
        private readonly UniverseEntity universe;

        private readonly string name;

        public CurrencyByName(UniverseEntity universe, string name)
        {
            Contract.Assert(universe != null);
            this.universe = universe;
            this.name = name;
        }

        public override Expression<Func<CurrencyEntity, bool>> IsSatisfiedBy()
        {
            return m =>
                   m.Universe.Id == this.universe.Id
                   && m.Name == this.name;
        }
    }
}