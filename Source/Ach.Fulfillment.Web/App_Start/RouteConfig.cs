namespace Ach.Fulfillment.Web.App_Start
{
    using System.Web.Mvc;
    using System.Web.Routing;

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // bind other to Error404
            routes.MapRoute("All", "{*catchall}", new { area = "Main", controller = "Error", action = "Error404" });
        }
    }
}