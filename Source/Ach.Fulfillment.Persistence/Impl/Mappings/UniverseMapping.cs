namespace Ach.Fulfillment.Persistence.Impl.Mappings
{
    using System;

    using Ach.Fulfillment.Data;

    using FluentNHibernate.Automapping;
    using FluentNHibernate.Automapping.Alterations;

    [Obsolete]
    public class UniverseMapping : IAutoMappingOverride<UniverseEntity>
    {
        public void Override(AutoMapping<UniverseEntity> mapping)
        {
        }
    }
}