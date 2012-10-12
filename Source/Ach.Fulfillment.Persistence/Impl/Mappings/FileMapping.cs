namespace Ach.Fulfillment.Persistence.Impl.Mappings
{
    using Data;
    using FluentNHibernate.Automapping;
    using FluentNHibernate.Automapping.Alterations;
    
    public class FileMapping : IAutoMappingOverride<FileEntity>
    {
        public void Override(AutoMapping<FileEntity> mapping)
        {
             mapping.Table("AchFile");
             mapping.Id(x => x.Id, "AchFileId").GeneratedBy.Identity();
             mapping.HasManyToMany(i => i.Transactions)
                .Table("AchFileTransaction")
                .ChildKeyColumn("AchTransactionId")
                .ParentKeyColumn("AchFileId")
                .LazyLoad();

             mapping.References(x => x.Partner, "PartnerId").Fetch.Join();
        }
    }
}
