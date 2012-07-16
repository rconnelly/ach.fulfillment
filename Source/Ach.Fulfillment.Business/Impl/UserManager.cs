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

        public UserEntity Load(long id)
        {
            var instance = this.Repository.Load<UserEntity>(id);
            return instance;
        }

        public UserEntity Create(UserEntity user, string password)
        {
            Contract.Assert(user != null);
            Contract.Assert(password != null);

            throw new System.NotImplementedException();
        }

        public IList<UserEntity> FindByLogin(string login)
        {
            throw new System.NotImplementedException();
        }

        public IList<UserEntity> FindByEmail(string email)
        {
            throw new System.NotImplementedException();
        }
    }
}