namespace Ach.Fulfillment.Persistence.Impl.Mappings
{
    using Data;
    using FluentNHibernate.Automapping;
    using FluentNHibernate.Automapping.Alterations;
    
    public class AchFileMapping : IAutoMappingOverride<AchFileEntity>
    {
        public void Override(AutoMapping<AchFileEntity> mapping)
        {
             mapping.Table("AchFile");
             mapping.Id(x => x.Id, "AchFileId").GeneratedBy.Identity();
             mapping.Map(x => x.FileStatus).CustomType<AchFileStatus>().Column("FileStatus").Not.Nullable();
             mapping.HasManyToMany(i => i.Transactions)
                .Table("AchFileTransaction")
                .ChildKeyColumn("AchTransactionId")
                .ParentKeyColumn("AchFileId")
                .LazyLoad().Cascade.AllDeleteOrphan();

             mapping.References(x => x.Partner, "PartnerId");
        }
    }
}
