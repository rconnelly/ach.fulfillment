namespace Ach.Fulfillment.Business
{
    using System.Collections.Generic;

    using Ach.Fulfillment.Data;

    public interface IUserManager
    {
        UserEntity Load(long id);

        UserEntity Create(UserEntity user, string password);

        IList<UserEntity> FindByLogin(string login);

        IList<UserEntity> FindByEmail(string email);
    }
}