namespace Ach.Fulfillment.Business.Security
{
    using System.Security.Principal;

    public interface IApplicationIdentity : IIdentity
    {
        string Login { get; }

        string Email { get; }

        string IpAddress { get; }
    }
}