using System;
using System.Net;
using System.Net.Http;
using System.Web.Helpers;
using System.Web.Mvc;
using Ach.Fulfillment.Business.Exceptions;
using Ach.Fulfillment.Web.Areas.Manage.Models;
using Ach.Fulfillment.Web.Common;
using Ach.Fulfillment.Web.Common.Controllers;

namespace Ach.Fulfillment.Web.Areas.Manage.Controllers
{
    public class AchTransactionController : Controller<AchManager>
    {
        #region Create
        [HttpPost]
        [AllowAnonymous]
       // [BusinessValidationFilter]
        public ActionResult Create(AchTransactionModel value)
        {
            if (this.ModelState.IsValid)
            {
                try
                {
                    var id = this.Manager.CreateAchTransaction(value);
                }
                catch (BusinessValidationException exc)
                {
                    this.ModelState.FillFrom(exc);
                }
            }
            var response = new HttpResponseMessage(HttpStatusCode.Created);
            return new JsonResult() { Data = response };//TODO put here some result
        }

        #endregion Create

        #region Generate

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Generate() //TODO remove, only for testing
        {
            var achFile = Manager.GenerateAchFiles();
            return Json(achFile,JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}
