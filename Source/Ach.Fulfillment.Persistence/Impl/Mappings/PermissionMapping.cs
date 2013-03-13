namespace Ach.Fulfillment.Persistence.Impl.Mappings
{
    using Ach.Fulfillment.Data;

    using FluentNHibernate.Automapping;
    using FluentNHibernate.Automapping.Alterations;

    public class PermissionMapping : IAutoMappingOverride<PermissionEntity>
    {
        public void Override(AutoMapping<PermissionEntity> mapping)
        {
            mapping.References(x => x.Role)
                .Fetch.Join();
        }
    }
}