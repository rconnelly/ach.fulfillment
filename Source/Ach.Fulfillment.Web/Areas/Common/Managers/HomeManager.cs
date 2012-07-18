namespace Ach.Fulfillment.Web.Areas.Common.Managers
{
    using Ach.Fulfillment.Common.Security;

    using Microsoft.Practices.Unity;

    public class HomeManager
    {
        [Dependency]
        public IApplicationPrincipal ApplicationPrincipal { get; set; }
    }
}