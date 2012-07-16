namespace Ach.Fulfillment.Business
{
    using System.Collections.Generic;

    using Ach.Fulfillment.Data;

    public interface IUserManager
    {
        UserEntry Load(long id);

        UserEntry Create(UserEntry user, string password);

        IList<UserEntry> FindByLogin(string login);

        IList<UserEntry> FindByEmail(string email);
    }
}