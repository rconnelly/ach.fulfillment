namespace Ach.Fulfillment.Web.Areas.Api.Controllers
{
    using System.Web.Mvc;

    using Ach.Fulfillment.Business.Exceptions;
    using Ach.Fulfillment.Common.Exceptions;
    using Ach.Fulfillment.Web.Areas.Api.Models;
    using Ach.Fulfillment.Web.Common.Controllers;
    using Ach.Fulfillment.Web.Common.Filters;

    [ApiValidationFilter]
    public class AchTransactionController : Controller<AchManager>
    {
        #region Methods

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Create(AchTransactionModel value)
        {
            var transactionId = this.Manager.CreateAchTransaction(value);
            return this.Json(new { transactionId });
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetStatus(long transactionId)
        {
            try
            {
                var transactionStatus = this.Manager.GetAchTransactionById(transactionId);
                return this.Json(transactionStatus, JsonRequestBehavior.AllowGet);
            }
            catch (ObjectNotFoundException ex)
            {
                throw new BusinessException("Transaction not found", ex);
            }
        }

        #endregion Methods
    }
}
