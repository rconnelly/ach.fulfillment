namespace Ach.Fulfillment.Persistence.Impl.Mappings
{
    using Data;
    using FluentNHibernate.Automapping;
    using FluentNHibernate.Automapping.Alterations;

    public class PartnerDetailMapping : IAutoMappingOverride<PartnerDetailEntity>
    {
        public void Override(AutoMapping<PartnerDetailEntity> mapping)
        {          
            mapping.References(x => x.Partner);
        }
    }
}
