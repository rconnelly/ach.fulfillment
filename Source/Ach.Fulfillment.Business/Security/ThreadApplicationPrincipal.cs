namespace Ach.Fulfillment.Business.Security
{
    using System.Diagnostics.Contracts;
    using System.Security.Principal;
    using System.Threading;

    using Ach.Fulfillment.Data;

    public class ThreadApplicationPrincipal : IApplicationPrincipal
    {
        #region Properties

        private static IApplicationPrincipal ParentPrincipal
        {
            get
            {
                var identity = Thread.CurrentPrincipal as IApplicationPrincipal;
                Contract.Assert(identity != null);
                return identity;
            }
        }

        #endregion

        public bool IsInRole(string role)
        {
            return ParentPrincipal.IsInRole(role);
        }

        public IIdentity Identity
        {
            get
            {
                return ParentPrincipal.Identity;
            }
        }

        public AccessRight[] AccessRights
        {
            get
            {
                return ParentPrincipal.AccessRights;
            }
        }

        public string RoleName
        {
            get
            {
                return ParentPrincipal.RoleName;
            }
        }

        public string Login
        {
            get
            {
                return ParentPrincipal.Login;
            }
        }

        public bool HasRight(AccessRight right)
        {
            return ParentPrincipal.HasRight(right);
        }
    }
}