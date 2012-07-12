namespace Ach.Fulfillment.Business.Impl
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    using Ach.Fulfillment.Business.Impl.Validation;
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications;

    using Microsoft.Practices.EnterpriseLibrary.Security.Cryptography;

    internal class UniverseManager : ManagerBase, IUniverseManager
    {
        #region Constants and Fields

        private const string HashInstance = "PasswordHashing";

        #endregion

        #region Public Methods and Operators

        public void ChangePassword(UniverseEntity instance, string newPassword)
        {
            Contract.Assert(instance != null);
            Contract.Assert(newPassword != null);
            instance.PasswordHash = Cryptographer.CreateHash(HashInstance, newPassword);
            this.DemandValid<UniverseValidator>(instance);
            this.Repository.Update(instance);
        }

        public UniverseEntity Create(string login, string password)
        {
            Contract.Assert(login != null);
            Contract.Assert(password != null);
            var hash = Cryptographer.CreateHash(HashInstance, password);
            var instance = new UniverseEntity { Login = login, PasswordHash = hash };
            this.DemandValid<UniverseValidator>(instance);
            this.Repository.Create(instance);
            return instance;
        }

        public void Delete(UniverseEntity instance)
        {
            Contract.Assert(instance != null);
            Contract.Assert(!instance.Deleted);
            instance.Deleted = true;
            this.Repository.Update(instance);
        }

        public UniverseEntity Find(string login, string password)
        {
            Contract.Assert(login != null);
            Contract.Assert(password != null);
            var universe = this.Repository.FindOne(new UniverseByActiveLogin(login));
            if (universe != null)
            {
                if (!Cryptographer.CompareHash(HashInstance, password, universe.PasswordHash))
                {
                    universe = null;
                }
            }

            return universe;
        }

        public UniverseEntity Load(long id)
        {
            var instance = this.Repository.Load<UniverseEntity>(id);
            return instance;
        }

        public IEnumerable<UniverseEntity> LoadAll(bool showDeleted)
        {
            var result = this.Repository.FindAll(new UniverseAll(showDeleted));
            return result;
        }

        #endregion
    }
}