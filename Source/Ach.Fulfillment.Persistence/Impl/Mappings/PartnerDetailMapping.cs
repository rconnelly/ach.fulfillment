using Ach.Fulfillment.Data;
using FluentNHibernate;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace Ach.Fulfillment.Persistence.Impl.Mappings
{
    public class PartnerDetailMapping : IAutoMappingOverride<PartnerDetailEntity>
    {
        public void Override(AutoMapping<PartnerDetailEntity> mapping)
        {
            mapping.Id(Reveal.Member<PartnerDetailEntity>("PartnerId"))
                .GeneratedBy.Foreign("Partner");
            mapping.HasOne(Reveal.Member<PartnerDetailEntity>("Partner"))
                .Constrained()
                .ForeignKey();
        }
    }
}
