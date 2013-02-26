namespace Ach.Fulfillment.Web.Areas.Main
{
    using System.Web.Mvc;

    using Ach.Fulfillment.Web.Common;

    public class MainAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get { return "Main"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(RouteHelper.Root, string.Empty, new { controller = "Home", action = "Index" });
            context.MapRoute(RouteHelper.Logon, "Login", new { controller = "Home", action = "Login" });

            context.MapRoute(
                "Main_default",
                "main/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional });
        }
    }
}
