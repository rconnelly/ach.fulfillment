namespace Ach.Fulfillment.Business
{
    using System.Collections.Generic;

    using Ach.Fulfillment.Data;

    public interface IPartnerManager
    {
        PartnerEntity Load(long id);

        IEnumerable<PartnerEntity> FindAll(bool withDisabled = false);

        PartnerEntity Create(PartnerEntity partner);

        void Delete(PartnerEntity partner);
        
        void Update(PartnerEntity partner);

        PartnerEntity AddUser(PartnerEntity partner, UserEntity user);
    }
}