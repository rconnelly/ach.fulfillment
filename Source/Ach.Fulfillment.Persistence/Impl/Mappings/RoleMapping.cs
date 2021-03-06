namespace Ach.Fulfillment.Persistence.Impl.Mappings
{
    using Ach.Fulfillment.Data;

    using FluentNHibernate.Automapping;
    using FluentNHibernate.Automapping.Alterations;

    public class RoleMapping : IAutoMappingOverride<RoleEntity>
    {
        public void Override(AutoMapping<RoleEntity> mapping)
        {
            mapping.HasMany(i => i.Permissions)
                .Inverse()
                .LazyLoad()
                .Cascade.All();
        }
    }
}