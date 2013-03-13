namespace Ach.Fulfillment.Persistence.Impl.Commands.Partner
{
    using System.Linq;

    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications.Partners;

    using NHibernate.Linq;

    internal class GetDefaultPartnerCommand : RelationalScalarQueryCommand<DefaultPartner, PartnerEntity>
    {
        public override PartnerEntity ExecuteScalar(DefaultPartner queryData)
        {
            var result =
                this.Session
                    .Query<PartnerEntity>()
                    .Fetch(p => p.Details)
                    .FirstOrDefault(m => m.Users.Any(u => u.Role.Name == MetadataInfo.DefaultUserRole));
            return result;
        }
    }
}