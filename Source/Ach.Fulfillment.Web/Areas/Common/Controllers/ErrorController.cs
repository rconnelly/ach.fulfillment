namespace Ach.Fulfillment.Web.Areas.Common.Controllers
{
    using System.Net;
    using System.Web.Mvc;

    public class ErrorController : Controller
    {
        [AllowAnonymous]
        public ActionResult Error404(string url)
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            
            return View("Error404");
        }

        public ActionResult Error403(string url)
        {
            Response.StatusCode = (int)HttpStatusCode.Forbidden;

            return View("Error403");
        }
    }
}
