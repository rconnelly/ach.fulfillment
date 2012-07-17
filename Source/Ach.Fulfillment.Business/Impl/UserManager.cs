namespace Ach.Fulfillment.Business.Impl
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Security.Cryptography;

    using Ach.Fulfillment.Business.Exceptions;
    using Ach.Fulfillment.Business.Impl.Validation;
    using Ach.Fulfillment.Common.Transactions;
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications;

    using Microsoft.Practices.EnterpriseLibrary.Security.Cryptography;

    internal class UserManager : ManagerBase<UserEntity>, IUserManager
    {
        #region Fields

        internal const string HashInstance = "PasswordHashing";

        #endregion

        #region Methods

        public override UserEntity Create(UserEntity instance)
        {
            throw new NotSupportedException("Try to use Create(instance,login,password) overload");
        } 

        public void Create(UserEntity instance, string login, string password)
        {
            Contract.Assert(instance != null);
            Contract.Assert(login != null);
            Contract.Assert(password != null);
            
            this.DemandValid<PasswordValidator>(password);
            var salt = GenerateRandomSalt();
            var paswordWithSalt = GetSaltedPassword(password, salt);
            var credential = new UserPasswordCredentialEntity
                {
                    Login = login, 
                    PasswordHash = Cryptographer.CreateHash(HashInstance, paswordWithSalt), 
                    PasswordSalt = salt,
                    User = instance
                };
            if (instance.UserPasswordCredentials == null)
            {
                instance.UserPasswordCredentials = new Collection<UserPasswordCredentialEntity>();
            }

            instance.UserPasswordCredentials.Add(credential);

            this.DemandValid<UserValidator>(instance);
            this.Repository.Create(instance);
        }

        public void ChangePassword(UserEntity instance, string newPassword)
        {
            Contract.Assert(instance != null);
            Contract.Assert(newPassword != null);

            var credential = instance.UserPasswordCredentials.FirstOrDefault();
            if (credential == null)
            {
                throw new BusinessValidationException("Password", "User does not have asssociated password credential");
            }

            this.DemandValid<PasswordValidator>(newPassword);
            var salt = GenerateRandomSalt();
            var paswordWithSalt = GetSaltedPassword(newPassword, salt);
            credential.PasswordHash = Cryptographer.CreateHash(HashInstance, paswordWithSalt);
            credential.PasswordSalt = salt;
            this.Repository.Update(credential);
        }

        public UserEntity FindByPasswordCredential(string login, string password)
        {
            Contract.Assert(login != null);
            Contract.Assert(password != null);
            UserEntity user = null;
            var credential = this.Repository.FindOne<UserPasswordCredentialEntity>(new UserPasswordCredentialByLogin(login));
            if (credential != null)
            {
                var saltedPassword = GetSaltedPassword(password, credential.PasswordSalt);
                if (Cryptographer.CompareHash(HashInstance, saltedPassword, credential.PasswordHash))
                {
                    user = credential.User;
                }
            }

            return user;
        }

        public override void Delete(UserEntity instance)
        {
            Contract.Assert(instance != null);
            using (var transaction = new Transaction())
            {
                instance.Deleted = true;
                instance.UserPasswordCredentials.Clear();
                this.Repository.Update(instance);
                this.Repository.Flush(true);

                transaction.Complete();
            }
        }

        private static string GetSaltedPassword(string password, string salt)
        {
            return salt + password;
        }

        private static string GenerateRandomSalt()
        {
            using (var cryptoProvider = new RNGCryptoServiceProvider())
            {
                var buff = new byte[15];
                cryptoProvider.GetBytes(buff);
                return Convert.ToBase64String(buff);
            }
        }

        #endregion
    }
}