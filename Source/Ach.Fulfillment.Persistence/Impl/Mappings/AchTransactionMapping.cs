using Ach.Fulfillment.Data;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace Ach.Fulfillment.Persistence.Impl.Mappings
{
    public class AchTransactionMapping : IAutoMappingOverride<AchTransactionEntity>
    {
        public void Override(AutoMapping<AchTransactionEntity> mapping)
        {
            mapping.References(x => x.Partner, "PartnerId").Cascade.None();
            mapping.Map(x => x.TransactionStatus).CustomType<int>().Column("TransactionStatus").Not.Nullable();

        }
    }
}
