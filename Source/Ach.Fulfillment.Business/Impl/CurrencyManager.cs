namespace Ach.Fulfillment.Business.Impl
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    using Ach.Fulfillment.Business.Impl.Validation;
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications;

    internal class CurrencyManager : ManagerBase, ICurrencyManager
    {
        public CurrencyEntity Create(UniverseEntity universe, CurrencyEntity currency)
        {
            Contract.Assert(universe != null);
            Contract.Assert(currency != null);
            currency.Id = 0;
            currency.Universe = universe;
            this.DemandValid<CurrencyValidator>(currency);
            this.Repository.Create(currency);
            return currency;
        }

        public CurrencyEntity Load(long id)
        {
            var result = this.Repository.Load<CurrencyEntity>(id);
            return result;
        }

        public CurrencyEntity Load(UniverseEntity universe, string name)
        {
            var currency = this.Repository.LoadOne(new CurrencyByName(universe, name));
            return currency;
        }

        public IEnumerable<CurrencyEntity> FindAll(UniverseEntity universe)
        {
            var currencies = this.Repository.FindAll(new CurrencyAll(universe));
            return currencies;
        }

        public void Update(CurrencyEntity currency)
        {
            Contract.Assert(currency != null);
            this.DemandValid<CurrencyValidator>(currency);
            this.Repository.Update(currency);
        }

        public void Delete(CurrencyEntity currency)
        {
            Contract.Assert(currency != null);
            this.Repository.Delete(currency);
        }
    }
}