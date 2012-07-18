namespace Ach.Fulfillment.Business.Security
{
    using System.Diagnostics.Contracts;

    public class ApplicationIdentity : IApplicationIdentity
    {
        #region Constructors and Destructors

        public ApplicationIdentity(string login, string name, string email)
            : this(login, name, email, "application")
        {
        }

        public ApplicationIdentity(string login, string name, string email, string type)
        {
            Contract.Assert(login != null);
            Contract.Assert(name != null);
            Contract.Assert(email != null);

            this.Name = name;
            this.Login = login;
            this.Email = email;
            this.AuthenticationType = type;
        }

        #endregion

        #region Public Properties

        public string AuthenticationType { get; private set; }

        public bool IsAuthenticated
        {
            get
            {
                return true;
            }
        }

        public string Name { get; private set; }

        public string Login { get; private set; }

        public string Email { get; private set; }

        public string IpAddress { get; set; }

        #endregion
    }
}
