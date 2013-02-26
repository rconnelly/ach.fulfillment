namespace Ach.Fulfillment.Web.Areas.Api
{
    using System.Web.Mvc;

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
                "TransactionCreation",
                "api/transaction", 
                new { controller = "AchTransaction", action = "Create" });

            context.MapRoute(
                "TransactionDetails", 
                "api/transaction/{transactionId}",
                new { controller = "AchTransaction", action = "GetStatus" },
                new { transactionId = @"\d+" });

            context.MapRoute(
                "Api_default",
                "api/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional });
        }
    }
}
