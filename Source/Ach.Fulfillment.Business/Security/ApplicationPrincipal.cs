namespace Ach.Fulfillment.Business.Security
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Security.Principal;

    using Ach.Fulfillment.Data;

    public class ApplicationPrincipal : IApplicationPrincipal
    {
        #region Constructors and Destructors

        public ApplicationPrincipal(IApplicationIdentity identity, PrincipalRole role)
        {
            Contract.Assert(identity != null);
            Contract.Assert(role != null);

            this.Identity = identity;
            this.Role = role;
        }

        #endregion

        #region Public Properties

        public AccessRight[] AccessRights
        {
            get
            {
                return this.Role.Rights;
            }
        }

        IIdentity IPrincipal.Identity
        {
            get
            {
                return this.Identity;
            }
        }

        public IApplicationIdentity Identity { get; private set; }

        public string RoleName
        {
            get
            {
                return this.Role.Name;
            }
        }

        public string Login
        {
            get
            {
                return this.Identity.Login;
            }
        }

        #endregion

        #region Properties

        private PrincipalRole Role { get; set; }

        #endregion

        #region Public Methods and Operators

        public bool HasRight(AccessRight right)
        {
            return this.AccessRights.Any(r => r == right);
        }

        public bool IsInRole(string role)
        {
            Contract.Assert(role != null);

            return this.RoleName.Equals(role, StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion
    }
}