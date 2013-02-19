namespace Ach.Fulfillment.Web.Areas.Manage.Controllers
{
    using System.Web.Mvc;

    using Ach.Fulfillment.Business.Exceptions;
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Web.Areas.Manage.Models;
    using Ach.Fulfillment.Web.Common;
    using Ach.Fulfillment.Web.Common.Controllers;
    using Ach.Fulfillment.Web.Common.Filters;

    public class AchTransactionController : Controller<AchManager>
    {
        #region Create
        [HttpPost]
        [AllowAnonymous]
        [BusinessValidationFilter]
        public ActionResult Create(AchTransactionModel value)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Manager.CreateAchTransaction(value);
                }
                catch (BusinessValidationException exc)
                {
                    ModelState.FillFrom(exc);
                }

                // TODO: why AchTransactionStatus.Received in case of BusinessValidationException
                return new JsonResult { Data = AchTransactionStatus.Received };
            }

            return new JsonResult { Data = ModelState };
        }

        #endregion Create
    }
}
