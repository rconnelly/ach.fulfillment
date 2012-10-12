namespace Ach.Fulfillment.Web.Areas.Manage.Controllers
{
    using System.Web.Mvc;
    using Business.Exceptions;
    using Data;
    using Models;
    using Common;
    using Common.Controllers;

    public class AchTransactionController : Controller<AchManager>
    {
        #region Create
        [HttpPost]
        [AllowAnonymous]
       // [BusinessValidationFilter]
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
            }
            return new JsonResult { Data = AchTransactionStatus.Received };
        }

        #endregion Create

    }
}
