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

        public long CreateAchTransaction(AchTransactionModel model)
        {
            var transaction = new AchTransactionEntity
                                  {
                                      MerchantName = model.Name,
                                      MerchantId = model.Id,
                                      MerchantDescription = model.Description,
                                      AccountId = model.AccountId,
                                      Amount = model.Amount,
                                      RoutingNumber = model.RoutingNumber,
                                      CallbackUrl = model.CallbackUrl,
                                      User = new UserEntity{Id = 1},//TODO remove or change to proper one
                                      IsQueued = true
                                  };

            this.Manager.Create(transaction);

            return transaction.Id;
        }

        //TODO this method is for testing only
        public string GenerateAchFiles()
        {
            var achfile= Manager.Generate();

            var achfilesStore = ConfigurationManager.AppSettings["AchFilesStore"];
                
            var newFileName = DateTime.UtcNow.ToString();
            var newPath = System.IO.Path.Combine(achfilesStore, "achfile.txt");

            var file = new System.IO.StreamWriter(newPath);
            file.Write("achfile");
            file.Flush();
            file.Close();
            return achfile;
        }
    }
}