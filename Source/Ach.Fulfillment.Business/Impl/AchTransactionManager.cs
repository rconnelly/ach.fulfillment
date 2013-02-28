namespace Ach.Fulfillment.Business.Impl
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using Ach.Fulfillment.Business.Impl.Validation;
    using Ach.Fulfillment.Common.Transactions;
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications;

    internal class AchTransactionManager : ManagerBase<AchTransactionEntity>, IAchTransactionManager
    {
        #region Public Methods and Operators

        public override AchTransactionEntity Create(AchTransactionEntity transaction)
        {
            Contract.Assert(transaction != null);

            this.DemandValid<AchTransactionValidator>(transaction);
            transaction.TransactionStatus = AchTransactionStatus.Received;

            using (var tx = new Transaction())
            {
                var instance = base.Create(transaction);
                this.OnTransactionCreated(instance);
                tx.Complete();
                return instance;
            }
        }

        public void UpdateStatus(AchTransactionStatus status, IList<AchTransactionEntity> transactions)
        {
            Contract.Assert(transactions != null);

            using (var tx = new Transaction())
            {
                foreach (var transactionEntity in transactions)
                {
                    transactionEntity.TransactionStatus = status;
                    this.Update(transactionEntity);
                }

                this.SendAchTransactionNotification(transactions);

                tx.Complete();
            }
        }

        public IEnumerable<AchTransactionEntity> GetEnqueued(PartnerEntity partner, bool toLock = true)
        {
            Contract.Assert(partner != null);

            var transactions = Repository.FindAll(new AchTransactionInQueueForPartner(partner)).ToList();
            if (toLock)
            {
                this.Lock(transactions);
            }

            return transactions;
        }

        public void Lock(IList<AchTransactionEntity> transactions)
        {
            Contract.Assert(transactions != null);

            using (var tx = new Transaction())
            {
                foreach (var achTransactionEntity in transactions)
                {
                    achTransactionEntity.Locked = true;
                    this.Update(achTransactionEntity);
                }

                tx.Complete();
            }
        }

        public void UnLock(IList<AchTransactionEntity> transactions)
        {
            Contract.Assert(transactions != null);

            using (var tx = new Transaction())
            {
                foreach (var achTransactionEntity in transactions)
                {
                    achTransactionEntity.Locked = false;
                    this.Update(achTransactionEntity);
                }

                tx.Complete();
            }
        }

        public void SendAchTransactionNotification(IList<AchTransactionEntity> transactions)
        {
            Contract.Assert(transactions != null);

            foreach (var achTransactionEntity in transactions)
            {
                ClientNotifier.NotificationRequest(
                    achTransactionEntity.CallbackUrl, achTransactionEntity.TransactionStatus.ToString()); // ToDo format notification
            }
        }

        private void OnTransactionCreated(AchTransactionEntity instance)
        {
            this.SendAchTransactionNotification(new List<AchTransactionEntity> { instance });
        }

        #endregion
    }
}