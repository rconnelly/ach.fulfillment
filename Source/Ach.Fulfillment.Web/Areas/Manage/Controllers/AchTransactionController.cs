using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;

namespace Ach.Fulfillment.Web.Areas.Manage.Controllers
{
    using System.Web.Mvc;

    using Ach.Fulfillment.Business.Exceptions;
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
                long transactionid = 0;

                try
                {
                    transactionid = Manager.CreateAchTransaction(value);
                }
                catch (BusinessValidationException exc)
                {
                    ModelState.FillFrom(exc);
                }
                return Json(new { transactionId = transactionid });
            }
            
            if (value.CallbackUrl != null)
                Manager.SendCallBack(value.CallbackUrl, JsonConvert.SerializeObject(ModelState));

            Response.StatusCode = (int) HttpStatusCode.BadRequest;
            return Content(JsonConvert.SerializeObject(ModelState), "application/json");
          
        }
        #endregion Create

        #region Get
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Get(long? id)
        {
            try
            {
                if(id == null)
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest));

                var achTransaction = Manager.LoadAchTransaction((long) id);

                if(achTransaction == null)
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));

                return Json( 
                new {
                    transactionId = achTransaction.AchTransactionId.ToString(), 
                    transactionStatus = achTransaction.Status
                }, JsonRequestBehavior.AllowGet);
                
            }
            catch (Exception)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }
        }

        #endregion Get
    }
}
