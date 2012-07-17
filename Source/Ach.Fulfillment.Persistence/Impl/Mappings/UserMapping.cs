namespace Ach.Fulfillment.Persistence.Impl.Mappings
{
    using Ach.Fulfillment.Data;

    using FluentNHibernate.Automapping;
    using FluentNHibernate.Automapping.Alterations;

    public class UserMapping : IAutoMappingOverride<UserEntity>
    {
        public void Override(AutoMapping<UserEntity> mapping)
        {
            mapping.Table("[User]");
            mapping.HasMany(i => i.UserPasswordCredentials).LazyLoad().Cascade.AllDeleteOrphan().Inverse();
            mapping.HasManyToMany(i => i.Partners).LazyLoad().Table("PartnerUser");
        }
    }
}