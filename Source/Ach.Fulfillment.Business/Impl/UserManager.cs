namespace Ach.Fulfillment.Business.Impl
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    using Ach.Fulfillment.Business.Impl.Validation;
    using Ach.Fulfillment.Data;

    using Microsoft.Practices.EnterpriseLibrary.Security.Cryptography;

    internal class UserManager : ManagerBase, IUserManager
    {
        private const string HashInstance = "PasswordHashing";

        public UserEntry Load(long id)
        {
            var instance = this.Repository.Load<UserEntry>(id);
            return instance;
        }

        public UserEntry Create(UserEntry user, string password)
        {
            Contract.Assert(user != null);
            Contract.Assert(password != null);

            var hash = Cryptographer.CreateHash(HashInstance, password);
            var instance = new UniverseEntity { Login = login, PasswordHash = hash };
            this.DemandValid<UniverseValidator>(instance);
            this.Repository.Create(instance);
            return instance;
        }

        public IList<UserEntry> FindByLogin(string login)
        {
            throw new System.NotImplementedException();
        }

        public IList<UserEntry> FindByEmail(string email)
        {
            throw new System.NotImplementedException();
        }
    }
}