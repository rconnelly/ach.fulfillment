namespace Ach.Fulfillment.Web.Areas.Api.Controllers
{
    using System;
    using System.Net;
    using System.Web;
    using System.Web.Mvc;

    using Ach.Fulfillment.Business.Exceptions;
    using Ach.Fulfillment.Web.Areas.Api.Models;
    using Ach.Fulfillment.Web.Common.Controllers;
    using Ach.Fulfillment.Web.Common.Filters;

    using Newtonsoft.Json;

    [ApiValidationFilter]
    public class AchTransactionController : Controller<AchManager>
    {
        #region Create

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
                // todo: remove all that try-catch-sendcallback crap
                if (value.CallbackUrl != null)
                {
                    this.Manager.SendCallBack(value.CallbackUrl, JsonConvert.SerializeObject(ex));
                }

                throw;
            }
        }

        #endregion Create

        #region Get

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Get(long? id)
        {
            try
            {
                // todo: why id is optional??
                if (id == null)
                {
                    throw new HttpException((int)HttpStatusCode.BadRequest, "id required");
                }

                var achTransaction = this.Manager.LoadAchTransaction(id.Value);

                // todo: why load method returns null
                if (achTransaction == null)
                {
                    throw new HttpException((int)HttpStatusCode.NotFound, "Transaction not found");
                }
                
                return this.Json(
                    new
                    {
                        transactionId = achTransaction.AchTransactionId.ToString(), 
                        transactionStatus = achTransaction.Status
                    }, 
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                // todo: why we are catching Exception and throwing NotFound?
                throw new HttpException((int)HttpStatusCode.NotFound, "???");
            }
        }

        #endregion Get
    }
}
