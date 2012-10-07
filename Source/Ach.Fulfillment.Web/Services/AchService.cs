using Ach.Fulfillment.Business;
using Ach.Fulfillment.Common;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ach.Fulfillment.Web.Services
{
    public class AchService:IAchService
    {
        #region Public Properties

        [Dependency]
        public IAchTransactionManager Manager { get; set; }

        #endregion

        #region Public Methods and Operators

        public string Generate()
        {
            return Manager.Generate();
        }

        #endregion
    }
}