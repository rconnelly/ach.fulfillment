using System;
using System.Configuration;
using Ach.Fulfillment.Data;
using Ach.Fulfillment.Web.Areas.Manage.Models;
using Microsoft.Practices.Unity;
using Ach.Fulfillment.Business;

namespace Ach.Fulfillment.Web.Areas.Manage.Controllers
{
    public class AchManager 
    {
        [Dependency]
        public IAchTransactionManager Manager { get; set; }

        [Dependency]
        public IPartnerManager PartnerManager { get; set;}
    

        public long CreateAchTransaction(AchTransactionModel model)
        {
            var partner = PartnerManager.Load(1);//TODO test version, change this

            var transaction = new AchTransactionEntity
                                  {
                                      ReceiverName = model.ReceiverName,
                                      IndividualIdNumber = model.IndividualIdNumber,
                                      EntryDescription = model.EntryDescription,
                                      DFIAccountId = model.DFIAccountId,
                                      Amount = model.Amount,
                                      TransitRoutingNumber = model.TransitRoutingNumber,
                                      CallbackUrl = model.CallbackUrl,
                                      TransactionCode = model.TransactionCode,
                                      EntryClassCode = model.EntryClassCode,
                                      EntryDate = model.EntryDate,
                                      ServiceClassCode = model.ServiceClassCode,
                                      PaymentRelatedInfo = model.PaymentRelatedInfo,
                                      Partner = partner,
                                      TransactionStatus = TransactionStatus.Received
                                  };

            this.Manager.Create(transaction);

            return transaction.Id;
        }

    }
}