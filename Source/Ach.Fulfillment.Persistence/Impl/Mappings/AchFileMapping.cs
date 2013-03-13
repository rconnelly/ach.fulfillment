namespace Ach.Fulfillment.Persistence.Impl.Mappings
{
    using Data;
    using FluentNHibernate.Automapping;
    using FluentNHibernate.Automapping.Alterations;
    
    public class AchFileMapping : IAutoMappingOverride<AchFileEntity>
    {
        public void Override(AutoMapping<AchFileEntity> mapping)
        {
            mapping.Map(x => x.FileStatus).CustomType<AchFileStatus>().Not.Nullable();
            mapping.HasMany(x => x.Transactions).LazyLoad().Cascade.AllDeleteOrphan();
            mapping.References(x => x.Partner);
        }
    }
}
