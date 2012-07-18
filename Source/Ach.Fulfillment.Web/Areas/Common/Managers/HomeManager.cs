namespace Ach.Fulfillment.Web.Areas.Common.Managers
{
    using Ach.Fulfillment.Business.Security;

    using Microsoft.Practices.Unity;

    public class HomeManager
    {
        [Dependency]
        public IApplicationPrincipal ApplicationPrincipal { get; set; }
    }
}