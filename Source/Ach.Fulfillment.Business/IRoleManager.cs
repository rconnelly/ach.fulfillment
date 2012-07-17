namespace Ach.Fulfillment.Business
{
    using System.Collections.Generic;

    using Ach.Fulfillment.Data;

    public interface IRoleManager : IManager<RoleEntity>
    {
        // todo: use cqrs
        IEnumerable<RoleEntity> FindAll();

        // todo: should not it be Find and cqrs?
        RoleEntity Load(string name);
    }
}