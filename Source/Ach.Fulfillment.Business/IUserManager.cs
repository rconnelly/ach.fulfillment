namespace Ach.Fulfillment.Business
{
    using Ach.Fulfillment.Data;

    public interface IUserManager : IManager<UserEntity>
    {
        void Create(UserEntity instance, string login, string password);

        void ChangePassword(UserEntity instance, string newPassword);

        UserEntity FindByPasswordCredential(string login, string password);
    }
}