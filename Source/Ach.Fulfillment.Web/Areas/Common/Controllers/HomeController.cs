namespace Ach.Fulfillment.Web.Areas.Common.Controllers
{
    using System.Web.Mvc;

    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Web.Areas.Common.Managers;
    using Ach.Fulfillment.Web.Common.Controllers;
    using Ach.Fulfillment.Web.Common.Filters;

    public class HomeController : Controller<HomeManager>
    {
        public ActionResult Index()
        {
            this.ViewBag.Message = "Modify this template to kick-start your ASP.NET MVC application.";

            return this.View();
        }

        [PrincipalRightPermission(AccessRight.Admin, AccessRight.SuperAdmin)]
        public ActionResult About()
        {
            this.ViewBag.Message = "Your app description page.";

            return this.View();
        }

        [PrincipalRightPermission(AccessRight.SuperAdmin)]
        public ActionResult Contact()
        {
            this.ViewBag.Message = "Your contact page.";

            return this.View();
        }
    }
}
