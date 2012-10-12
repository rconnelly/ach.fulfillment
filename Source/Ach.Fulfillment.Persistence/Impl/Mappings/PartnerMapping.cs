namespace Ach.Fulfillment.Persistence.Impl.Mappings
{
    using Data;

    using FluentNHibernate.Automapping;
    using FluentNHibernate.Automapping.Alterations;

    public class PartnerMapping : IAutoMappingOverride<PartnerEntity>
    {
        public void Override(AutoMapping<PartnerEntity> mapping)
        {
            mapping.HasManyToMany(i=>i.Users)
                .Table("PartnerUser")
                .ChildKeyColumn("UserId")
                .ParentKeyColumn("PartnerId")
                .LazyLoad();
            mapping.HasOne(i=>i.Details).PropertyRef(r=>r.Partner).Cascade.All();            
        }
    }
}