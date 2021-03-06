namespace Ach.Fulfillment.Business.Impl
{
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;

    using Ach.Fulfillment.Business.Impl.Validation;
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications.Partners;

    internal class PartnerManager : ManagerBase<PartnerEntity>, IPartnerManager
    {
        public override PartnerEntity Create(PartnerEntity partner)
        {
            Contract.Assert(partner != null);

            this.DemandValid<PartnerValidator>(partner);
            return base.Create(partner);
        }

        public override void Delete(PartnerEntity partner)
        {
            Contract.Assert(partner != null);
            Repository.Delete(partner);
        }

        public void Disable(PartnerEntity partner)
        {
            Contract.Assert(partner != null);
            Contract.Assert(!partner.Disabled);

            partner.Disabled = true;

            Repository.Update(partner);
        }

        public override void Update(PartnerEntity partner, bool flush = false)
        {
            Contract.Assert(partner != null);
            this.DemandValid<PartnerValidator>(partner);
            base.Update(partner, flush);
        }

        public PartnerEntity AddUser(PartnerEntity partner, UserEntity user)
        {
            Contract.Assert(partner != null);
            Contract.Assert(partner.Id > 0);
            Contract.Assert(user != null);
            Contract.Assert(user.Id > 0);

            if (partner.Users == null)
            {
                partner.Users = new Collection<UserEntity>
                    {
                        user
                    };
            }

            Repository.Update(partner);

            return partner;
        }

        public PartnerEntity GetDefault()
        {
            var partner = this.Repository.FindOne(new DefaultPartner());
            return partner;
        }
    }
}