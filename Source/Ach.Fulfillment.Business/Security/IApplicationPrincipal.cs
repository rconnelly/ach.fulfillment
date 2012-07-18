namespace Ach.Fulfillment.Business.Security
{
    using System.Security.Principal;

    using Ach.Fulfillment.Data;

    public interface IApplicationPrincipal : IPrincipal
    {
        AccessRight[] AccessRights { get; }

        string RoleName { get; }

        string Login { get; }

        bool HasRight(AccessRight right);
    }
}