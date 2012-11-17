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
            var entity = base.Create(transaction);
            this.SendAchTransactionNotification(new List<AchTransactionEntity> { entity });
            return entity;
        }
        
        public void ChangeAchTransactionStatus(List<AchTransactionEntity> transactions, AchTransactionStatus status)
        {
            Contract.Assert(transactions != null);

            using (var tx = new Transaction())
            {
                foreach (var achTransactionEntity in transactions)
                {
                    achTransactionEntity.TransactionStatus = status;
                    this.Update(achTransactionEntity);
                }

                tx.Complete();
            }

            this.SendAchTransactionNotification(transactions);
        }

        public void SendAchTransactionNotification(List<AchTransactionEntity> transactions)
        {
            Contract.Assert(transactions != null);
            foreach (var achTransactionEntity in transactions)
            {
                ClientNotifier.NotificationRequest(
                    achTransactionEntity.CallbackUrl, achTransactionEntity.TransactionStatus.ToString()); // ToDo format notification
            }
        }

        public List<AchTransactionEntity> GetTransactionsInQueue(bool toLock = true)
        {
            List<AchTransactionEntity> transactions;

            using (var tx = new Transaction())
            {
                transactions = Repository.FindAll(new AchTransactionInQueue()).ToList();

                foreach (var achTransactionEntity in transactions)
                {
                    achTransactionEntity.Locked = toLock;
                    this.Update(achTransactionEntity);
                }

                tx.Complete();
            }

            return transactions;
        }

        public void UnLockTransactions(List<AchTransactionEntity> transactions)
        {
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

        #endregion
    }
}