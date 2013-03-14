namespace Ach.Fulfillment.Tests.Business
{
    using System.Diagnostics;

    using Ach.Fulfillment.Business;
    using Ach.Fulfillment.Business.Impl.Validation;
    using Ach.Fulfillment.Common;
    using Ach.Fulfillment.Persistence;
    using Ach.Fulfillment.Tests.Common;

    using Microsoft.Practices.ServiceLocation;

    using NHibernate;

    using NUnit.Framework;

    [TestFixture]
    public class General : TestBase
    {
        public IServiceLocator Locator
        {
            get
            {
                return ServiceLocator.Current;
            }
        }

        public ISession Session { get; set; }

        public IUserManager UserManager 
        {
            get
            {
                return this.Locator.GetInstance<IUserManager>();
            } 
        }

        public IPartnerManager PartnerManager
        {
            get
            {
                return this.Locator.GetInstance<IPartnerManager>();
            }
        }

        public IAchTransactionManager AchTransactionManager
        {
            get
            {
                return this.Locator.GetInstance<IAchTransactionManager>();
            }
        }

        public IAchFileManager AchFileManager
        {
            get
            {
                return this.Locator.GetInstance<IAchFileManager>();
            }
        }

        public ICallbackNotificationManager CallbackNotificationManager
        {
            get
            {
                return this.Locator.GetInstance<ICallbackNotificationManager>();
            }
        }

        public IRepository Repository
        {
            get
            {
                return this.Locator.GetInstance<IRepository>();
            }
        }

        [Test]
        public void Validate()
        {
            var transaction = EntityHelper.CreateTestAchTransaction(null);
            transaction.Partner = EntityHelper.CreateTestPartner(null);
            var validator = new AchTransactionValidator();
            var validationResult = validator.Validate(transaction);
            Assert.That(validationResult.IsValid);

            transaction.TransactionCode = 74;
            validationResult = validator.Validate(transaction);
            Assert.That(validationResult.IsValid, Is.False);
            foreach (var failure in validationResult.Errors)
            {
                Trace.WriteLine(failure.ErrorMessage);
            }
        }

        [Test]
        public void Default()
        {
            /*using (new UnitOfWork())
            {
                var t = EntityHelper.CreateTestAchTransaction(null);
                
                var f = EntityHelper.CreateTestAchFile(null);
                f.Partner = this.PartnerManager.Load(2);
                f.Transactions.Add(t);
                f.Transactions.Add(t);
                f.Transactions.Add(t);
                f.Transactions.Add(t);

                var r = f.ToNacha(f.Transactions);

                r = r;
            }

            return;*/
            
            // - obtain ach transaction from client
            using (new UnitOfWork())
            {
                var partner = this.PartnerManager.GetDefault();
                var achTransaction = EntityHelper.CreateTestAchTransaction(null);
                achTransaction.Partner = partner;
                this.AchTransactionManager.Create(achTransaction);
            }

            // - group several ach transactions into one ach batch file
            using (new UnitOfWork())
            {
                this.AchFileManager.ProcessReadyToBeGroupedAchTransactions();
            }

            // - generate nacha file from ach batch file
            using (new UnitOfWork())
            {
                this.AchFileManager.ProcessReadyToBeGeneratedAchFile();
            }

            // - send nacha file to remote ftp server
            using (new UnitOfWork())
            {
                this.AchFileManager.ProcessReadyToBeUploadedAchFile();
            }

            // - get nacha file status from remote ftp server
            using (new UnitOfWork())
            {
                this.AchFileManager.ProcessReadyToBeAcceptedAchFile();
            }

            // - clean old data
            using (new UnitOfWork())
            {
                this.CallbackNotificationManager.DeliverRemoteNotifications();
            }

            // - clean old data
            using (new UnitOfWork())
            {
                this.AchFileManager.Cleanup();
            }
        }
    }
}
