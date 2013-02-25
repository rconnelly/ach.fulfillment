namespace Ach.Fulfillment.Web.Areas.Api.Controllers
{
    using System.Web.Mvc;

    using Ach.Fulfillment.Business.Exceptions;
    using Ach.Fulfillment.Common.Exceptions;
    using Ach.Fulfillment.Web.Areas.Api.Models;
    using Ach.Fulfillment.Web.Common.Controllers;
    using Ach.Fulfillment.Web.Common.Filters;

    using Newtonsoft.Json;

    [ApiValidationFilter]
    public class AchTransactionController : Controller<AchManager>
    {
        #region Methods

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Create(AchTransactionModel value)
        {
            try
            {
                var transactionId = this.Manager.CreateAchTransaction(value);
                return this.Json(new { transactionId });
            }
            catch (BusinessException ex)
            {
                // todo (ng): remove all that try-catch-sendcallback. we do not need any callbacks here
                if (value.CallbackUrl != null)
                {
                    this.Manager.SendCallBack(value.CallbackUrl, JsonConvert.SerializeObject(ex));
                }

                throw;
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetStatus(long transactionId)
        {
            try
            {
                var transactionStatus = this.Manager.LoadAchTransactionStatus(transactionId);
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
