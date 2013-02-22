namespace Ach.Fulfillment.Web.Areas.Api
{
    using System.Web.Mvc;

    using Ach.Fulfillment.Web.Common;

    public class ApiAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Api";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                RouteHelper.Transaction,
                "Api/Transaction", 
                new { area = "Api", controller = "AchTransaction", action = "Create" });

            context.MapRoute(
                "TransactionDetails", 
                "Api/Transaction/{id}",
                new { area = "Manage", controller = "AchTransaction", action = "Get", id = UrlParameter.Optional });

            context.MapRoute(
                "Api_default",
                "Api/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional });
        }
    }
}
