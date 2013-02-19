namespace Ach.Fulfillment.Web.Areas.Api.Controllers
{
    using System.Web.Mvc;

    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Web.Areas.Api.Models;
    using Ach.Fulfillment.Web.Common.Controllers;
    using Ach.Fulfillment.Web.Common.Filters;

    [ApiValidationFilter]
    public class AchTransactionController : Controller<AchManager>
    {
        #region Create

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Create(AchTransactionModel value)
        {
            this.Manager.CreateAchTransaction(value);
            return new JsonResult { Data = AchTransactionStatus.Received };
        }

        #endregion Create
    }
}
