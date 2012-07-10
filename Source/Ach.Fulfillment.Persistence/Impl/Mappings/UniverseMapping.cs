namespace Ach.Fulfillment.Persistence.Impl.Mappings
{
    using Ach.Fulfillment.Data;

    using FluentNHibernate.Automapping;
    using FluentNHibernate.Automapping.Alterations;

    public class UniverseMapping : IAutoMappingOverride<UniverseEntity>
    {
        public void Override(AutoMapping<UniverseEntity> mapping)
        {
        }
    }
}