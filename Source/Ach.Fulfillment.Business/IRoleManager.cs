namespace Ach.Fulfillment.Business
{
    using Ach.Fulfillment.Data;

    public interface IRoleManager : IManager<RoleEntity>
    {
        RoleEntity Load(string name);
    }
}