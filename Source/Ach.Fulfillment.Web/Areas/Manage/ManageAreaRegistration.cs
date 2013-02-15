using System.Web.Http;
using Ach.Fulfillment.Web.Common;

namespace Ach.Fulfillment.Web.Areas.Manage
{
    using System.Web.Mvc;

    public class ManageAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Manage";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                RouteHelper.Transaction, "Transaction",
                new { area = "Manage", controller = "AchTransaction", action = "Create" });

            context.MapRoute(
                "Transaction_default", "Transaction/{id}",
                new { area = "Manage", controller = "AchTransaction", action = "Get", id = UrlParameter.Optional });

            context.MapRoute(
                "Manage_default",
                "Manage/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional });
        }
    }
}
