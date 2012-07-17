namespace Ach.Fulfillment.Business
{
    using Ach.Fulfillment.Data;

    public interface IPartnerManager : IManager<PartnerEntity>
    {
        void Disable(PartnerEntity partner);
        
        PartnerEntity AddUser(PartnerEntity partner, UserEntity user);
    }
}