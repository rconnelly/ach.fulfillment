namespace Ach.Fulfillment.Business.Impl
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;

    using Ach.Fulfillment.Business.Impl.Validation;
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications;

    internal class PartnerManager : ManagerBase, IPartnerManager
    {
        public PartnerEntity Load(long id)
        {
            var instance = this.Repository.Load<PartnerEntity>(id);
            return instance;
        }

        public IEnumerable<PartnerEntity> FindAll(bool withDisabled = false)
        {
            var partners = this.Repository.FindAll(new PartnerAll(withDisabled));
            return partners;
        }

        public PartnerEntity Create(PartnerEntity partner)
        {
            Contract.Assert(partner != null);

            this.DemandValid<PartnerValidator>(partner);
            this.Repository.Create(partner);

            return partner;
        }

        public void Delete(PartnerEntity partner)
        {
            Contract.Assert(partner != null);
            Contract.Assert(!partner.Disabled);

            partner.Disabled = true;
            this.Repository.Update(partner);
        }

        public void Update(PartnerEntity partner)
        {
            Contract.Assert(partner != null);
            this.DemandValid<PartnerValidator>(partner);
            this.Repository.Update(partner);
        }

        public PartnerEntity AddUser(PartnerEntity partner, UserEntity user)
        {
            Contract.Assert(partner != null);
            Contract.Assert(user != null);
            Contract.Assert(user.Id > 0);

            if (partner.Users == null)
            {
                partner.Users = new Collection<UserEntity>
                    {
                        user
                    };
            }

            this.Repository.Update(partner);

            return partner;
        }
    }
}