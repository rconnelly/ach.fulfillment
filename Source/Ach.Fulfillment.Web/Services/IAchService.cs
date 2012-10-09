using Ach.Fulfillment.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ach.Fulfillment.Web.Services
{
    public interface IAchService
    {
        IAchTransactionManager Manager { get; set; }
        void Generate();
    }
}