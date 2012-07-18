namespace Ach.Fulfillment.Web.Areas.Common
{
    using System.Web.Mvc;

    using Ach.Fulfillment.Web.Common;

    public class CommonAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Common";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(RouteHelper.RouteRoot, string.Empty, new { area = "Common", controller = "Home", action = "Index" });
            context.MapRoute(RouteHelper.RouteLogon, "Login", new { area = "Common", controller = "Account", action = "Login" });

            context.MapRoute(
                "Common_default",
                "Common/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
