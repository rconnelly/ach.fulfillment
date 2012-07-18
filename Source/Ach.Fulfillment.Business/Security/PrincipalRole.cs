namespace Ach.Fulfillment.Business.Security
{
    using Ach.Fulfillment.Data;

    public class PrincipalRole
    {
        #region Constructors and Destructors

        public PrincipalRole(string name, AccessRight[] rights)
        {
            this.Name = name;
            this.Rights = rights;
        }

        #endregion

        #region Public Properties

        public string Name { get; private set; }

        public AccessRight[] Rights { get; private set; }

        #endregion
    }
}