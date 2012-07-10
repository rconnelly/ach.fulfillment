namespace Ach.Fulfillment.Business
{
    using System.Collections.Generic;

    using Ach.Fulfillment.Data;

    public interface IUniverseManager
    {
        UniverseEntity Create(string login, string password);

        UniverseEntity Load(long id);

        IEnumerable<UniverseEntity> LoadAll(bool showDeleted);

        UniverseEntity Find(string login, string password);

        void Delete(UniverseEntity instance);

        void ChangePassword(UniverseEntity instance, string newPassword);
    }
}
