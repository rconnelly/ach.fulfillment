namespace Ach.Fulfillment.Business
{
    using System.Collections.Generic;

    using Ach.Fulfillment.Data;

    public interface IPartnerManager : IManager<PartnerEntity>
    {
        // todo: cqrs
        IEnumerable<PartnerEntity> FindAll(bool withDisabled = false);

        PartnerEntity AddUser(PartnerEntity partner, UserEntity user);
    }
}