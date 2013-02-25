namespace Ach.Fulfillment.Web.Areas.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics.Contracts;

    using Ach.Fulfillment.Business;
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Web.Areas.Api.Models;

    using AutoMapper;

    using Microsoft.Practices.Unity;

    public class AchManager
    {
        #region Public Properties

        [Dependency]
        public IAchTransactionManager Manager { get; set; }

        [Dependency]
        public IUserManager UserManager { get; set; }

        #endregion

        #region Public Methods and Operators

        public long CreateAchTransaction(AchTransactionModel model)
        {
            Contract.Assert(model != null);

            // todo (ng): should not it be configurated in database?
            var login = ConfigurationManager.AppSettings["DefaultUser"];
            var user = this.UserManager.FindByLogin(login);
            Contract.Assert(user != null);
            var partner = user.Partner;

            var transaction = Mapper.Map<AchTransactionModel, AchTransactionEntity>(model);
            transaction.Partner = partner;

            // todo (ng): why do you need to repeat prefix "Transaction"
            // todo (ng): why we setting status here and not in business layer
            transaction.TransactionStatus = AchTransactionStatus.Received;

            this.Manager.Create(transaction);

            // todo (ng): why do we send notification from presentation
            this.Manager.SendAchTransactionNotification(new List<AchTransactionEntity> { transaction });

            return transaction.Id;
        }

        public AchTransactionStatusModel LoadAchTransactionStatus(long transactionId)
        {
            var achEntity = this.Manager.Load(transactionId);
            var achModel = new AchTransactionStatusModel
                               {
                                    AchTransactionId = achEntity.Id,
                                    Status = achEntity.TransactionStatus.ToString()
                               };
            return achModel;
        }

        [Obsolete("todo (ng): Method should not be here")]
        public void SendCallBack(string url, string data)
        {
            this.Manager.SendCallBack(url, data);
        }

        #endregion
    }
}