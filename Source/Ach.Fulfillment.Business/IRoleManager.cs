namespace Ach.Fulfillment.Business
{
    using System.Collections.Generic;

    using Ach.Fulfillment.Data;

    public interface IRoleManager
    {
        RoleEntity Create(RoleEntity role);

        IEnumerable<RoleEntity> FindAll();

        RoleEntity Load(long id);

        RoleEntity Load(string name);

        void Delete(RoleEntity role);

        void Update(RoleEntity currency);
    }
}