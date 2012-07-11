namespace Ach.Fulfillment.Business
{
    using System.Collections.Generic;

    using Ach.Fulfillment.Data;

    public interface ICurrencyManager
    {
        CurrencyEntity Create(UniverseEntity universe, CurrencyEntity currency);

        CurrencyEntity Load(long id);

        CurrencyEntity Load(UniverseEntity universe, string name);

        IEnumerable<CurrencyEntity> FindAll(UniverseEntity universe);

        void Update(CurrencyEntity currency);

        void Delete(CurrencyEntity currency);
    }
}