namespace Ach.Fulfillment.Persistence.Impl.Mappings
{
    using Data;
    using FluentNHibernate.Automapping;
    using FluentNHibernate.Automapping.Alterations;

    public class AchTransactionMapping : IAutoMappingOverride<AchTransactionEntity>
    {
        public void Override(AutoMapping<AchTransactionEntity> mapping)
        {
            mapping.References(x => x.Partner, "PartnerId").Cascade.None();
            mapping.Map(x => x.Status).CustomType<AchTransactionStatus>().Column("TransactionStatus").Not.Nullable();
            mapping.Map(x => x.NotifiedStatus).CustomType<AchTransactionStatus>().Column("NotifiedTransactionStatus").Not.Nullable();
        }
    }
}
