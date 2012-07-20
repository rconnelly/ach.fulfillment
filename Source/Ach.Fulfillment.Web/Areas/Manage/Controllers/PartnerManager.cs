namespace Ach.Fulfillment.Web.Areas.Manage.Controllers
{
    using Ach.Fulfillment.Business;

    using Microsoft.Practices.Unity;

    public class PartnerManager
    {
        [Dependency]
        public IPartnerManager Manager { get; set; }
    }
}