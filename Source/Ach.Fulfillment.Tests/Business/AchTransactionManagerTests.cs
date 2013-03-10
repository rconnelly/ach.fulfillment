namespace Ach.Fulfillment.Tests.Business
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using Ach.Fulfillment.Business;
    using Ach.Fulfillment.Business.Exceptions;
    using Ach.Fulfillment.Data;

    using FluentNHibernate.Testing;

    using Microsoft.Practices.Unity;

    using NUnit.Framework;

    using Rhino.Mocks;

    [TestFixture]
    public class AchTransactionManagerTests : BusinessIntegrationTestBase
    {
        #region Public Methods and Operators
       
        [Test]
        public void CreateTest()
        {
            var manager = Locator.GetInstance<IAchTransactionManager>();
            var partnerManager = Locator.GetInstance<IPartnerManager>();
            
            var partner = this.CreateTestPartner();
            partnerManager.Create(partner);
            var transaction = this.CreateTestAchTransaction();
            transaction.Partner = partner;
            
            var instance = manager.Create(transaction);
            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.Id, Is.GreaterThan(0));         
        }

        [Test]
        public void CreateAchTransactionUsingInvalidDataTest()
        {
            Trace.WriteLine("EntryClassCode test");
            var ex = Assert.Throws<BusinessValidationException>(() => this.CreateAchTransaction(entryClassCode: "QQQq"));
            Trace.WriteLine(ex.Message);
            Assert.That(ex.Errors.Count(), Is.EqualTo(2));

            Trace.WriteLine("CallbackUrl test");
            ex = Assert.Throws<BusinessValidationException>(() => this.CreateAchTransaction(string.Empty));
            Trace.WriteLine(ex.Message);
            Assert.That(ex.Errors.Count(), Is.EqualTo(3));

            Trace.WriteLine("CallbackUrl test");
            ex = Assert.Throws<BusinessValidationException>(() => this.CreateAchTransaction(new string('&', MetadataInfo.StringLong)));
            Trace.WriteLine(ex.Message);
            Assert.That(ex.Errors.Count(), Is.EqualTo(2));

            Trace.WriteLine("DfiAccountId test");
            ex = Assert.Throws<BusinessValidationException>(() => this.CreateAchTransaction(dfiAccountId: "QQqq#$&"));
            Trace.WriteLine(ex.Message);
            Assert.That(ex.Errors.Count(), Is.EqualTo(2));

            Trace.WriteLine("EntryDescription test");
            ex = Assert.Throws<BusinessValidationException>(() => this.CreateAchTransaction(entryDescription: "aass^7_@#bnm"));
            Trace.WriteLine(ex.Message);
            Assert.That(ex.Errors.Count(), Is.EqualTo(2));

            Trace.WriteLine("IndividualIdNumber test");
            ex = Assert.Throws<BusinessValidationException>(() => this.CreateAchTransaction(individualIdNumber: new string('&', MetadataInfo.StringShort)));
            Trace.WriteLine(ex.Message);
            Assert.That(ex.Errors.Count(), Is.EqualTo(2));

            Trace.WriteLine("IndividualIdNumber test");
            ex = Assert.Throws<BusinessValidationException>(() => this.CreateAchTransaction(individualIdNumber: new string('&', MetadataInfo.StringTiny)));
            Trace.WriteLine(ex.Message);
            Assert.That(ex.Errors.Count(), Is.EqualTo(2));

            Trace.WriteLine("PaymentRelatedInfo test");
            ex = Assert.Throws<BusinessValidationException>(() => this.CreateAchTransaction(paymentRelatedInfo: new string('$', MetadataInfo.StringNormal)));
            Trace.WriteLine(ex.Message);
            Assert.That(ex.Errors.Count(), Is.EqualTo(1));

            Trace.WriteLine("ReceiverName test");
            ex = Assert.Throws<BusinessValidationException>(() => this.CreateAchTransaction(receiverName: new string('$', MetadataInfo.StringShort + 3)));
            Trace.WriteLine(ex.Message);
            Assert.That(ex.Errors.Count(), Is.EqualTo(2));

            Trace.WriteLine("ServiceClassCode test");
            ex = Assert.Throws<BusinessValidationException>(() => this.CreateAchTransaction(serviceClassCode: "2211"));
            Trace.WriteLine(ex.Message);
            Assert.That(ex.Errors.Count(), Is.EqualTo(2));

            Trace.WriteLine("TransactionCode test");
            ex = Assert.Throws<BusinessValidationException>(() => this.CreateAchTransaction(transactionCode: "3333"));
            Trace.WriteLine(ex.Message);
            Assert.That(ex.Errors.Count(), Is.EqualTo(2));

            Trace.WriteLine("TransitRoutingNumber test");
            ex = Assert.Throws<BusinessValidationException>(() => this.CreateAchTransaction(transitRoutingNumber: new string('&', MetadataInfo.StringShort)));
            Trace.WriteLine(ex.Message);
            Assert.That(ex.Errors.Count(), Is.EqualTo(2));
        }

        [Test]
        [Ignore]
        public void CallCreateWithNullArgumentTest()
        {
            var manager = Locator.GetInstance<IAchTransactionManager>();

            Assert.Throws<ArgumentNullException>(() => manager.Create(null));
        }

        [Test]
        public void LoadTest()
        {
            var manager = Locator.GetInstance<IAchTransactionManager>();
            var partnerManager = Locator.GetInstance<IPartnerManager>();

            var partner = this.CreateTestPartner();
            partnerManager.Create(partner);
            var transaction = this.CreateTestAchTransaction();
            transaction.Partner = partner;
            var instance = manager.Create(transaction);

            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.Id, Is.GreaterThan(0));
           
            this.ClearSession(instance);

            transaction = manager.Load(instance.Id);
            Assert.That(transaction, Is.Not.Null);
        }

        /*[Test]
        public void ChangeAchTransactionStatusTest()
        {
            var manager = Locator.GetInstance<IAchTransactionManager>();
            var partnerManager = Locator.GetInstance<IPartnerManager>();

            var partner = this.CreateTestPartner();
            partnerManager.Create(partner);

            var transaction = this.CreateTestAchTransaction();
            transaction.Partner = partner;

            var instance = manager.Create(transaction);

            var transactions = new List<AchTransactionEntity> { transaction };
            manager.UpdateStatus(AchTransactionStatus.Batched, transactions);

            var changedTransaction = manager.Load(instance.Id);
            Assert.AreEqual(changedTransaction.Status, AchTransactionStatus.Batched);
        }*/

        [Test]
        [Ignore]
        public void SendNotificationWhenAchTransactionCreatedTest()
        {
            var manager = Locator.GetInstance<IAchTransactionManager>();
            var partnerManager = Locator.GetInstance<IPartnerManager>();

            var mocks = new MockRepository();
            var container = mocks.StrictMock<IUnityContainer>();
            var notifier = mocks.DynamicMock<IClientNotifier>();
            container.RegisterInstance(notifier);

            var notificationRequestWasCalled = false;

            var partner = this.CreateTestPartner();
            partnerManager.Create(partner);
            var transaction = this.CreateTestAchTransaction();
            transaction.Partner = partner;

            notifier.Expect(_ => notifier.NotificationRequest(transaction.CallbackUrl, transaction.Status.ToString()))
                .WhenCalled(delegate { notificationRequestWasCalled = true; });

            // mocks.ReplayAll();
            var instance = manager.Create(transaction);
            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.Id, Is.GreaterThan(0));
            Assert.IsTrue(notificationRequestWasCalled);
        }

        /*[Test]
        public void GetTransactionsInQueueTest()
        {
            var manager = Locator.GetInstance<IAchTransactionManager>();
            var partnerManager = Locator.GetInstance<IPartnerManager>();

            var partner = this.CreateTestPartner();
            partnerManager.Create(partner);

            var transaction = this.CreateTestAchTransaction();
            transaction.Partner = partner;
            var instance = manager.Create(transaction);

            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.Id, Is.GreaterThan(0));

            var transaction2 = this.CreateTestAchTransaction();
            transaction2.Partner = partner;
            var instance2 = manager.Create(transaction2);

            Assert.That(instance2, Is.Not.Null);
            Assert.That(instance2.Id, Is.GreaterThan(0));
            manager.UpdateStatus(AchTransactionStatus.Batched, transaction2);

            this.ClearSession(instance);
            this.ClearSession(instance2);

            var trns = manager.GetEnqueued(partner).ToList();
            Assert.That(trns, Is.Not.Null);
            Assert.AreEqual(trns.Count(), 1);
            Assert.AreEqual(trns.First().Status, AchTransactionStatus.Created);
            Assert.IsTrue(trns.First().Locked);
        }*/

        /*[Test]
        public void GetAllInQueueForPartnerTest()
        {
            var manager = Locator.GetInstance<IAchTransactionManager>();
            var partnerManager = Locator.GetInstance<IPartnerManager>();

            var partner = this.CreateTestPartner();
            partnerManager.Create(partner);

            var transaction = this.CreateTestAchTransaction();
            transaction.Partner = partner;
            var instance = manager.Create(transaction);

            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.Id, Is.GreaterThan(0));

            var partner2 = this.CreateTestPartner();
            partnerManager.Create(partner2);

            var transaction2 = this.CreateTestAchTransaction();
            transaction2.Partner = partner2;
            var instance2 = manager.Create(transaction2);

            Assert.That(instance2, Is.Not.Null);
            Assert.That(instance2.Id, Is.GreaterThan(0));

            this.ClearSession(instance);
            this.ClearSession(instance2);

            var trns = manager.GetEnqueued(partner).ToList();
            Assert.That(trns, Is.Not.Null);
            Assert.AreEqual(trns.Count(), 1);
            Assert.AreEqual(trns[0].Status, AchTransactionStatus.Created);
            Assert.IsTrue(trns[0].Locked);
            Assert.AreEqual(partner, trns[0].Partner);
        }*/

        /*[Test]
        public void UnLockTest()
        {
            var manager = Locator.GetInstance<IAchTransactionManager>();
            var partnerManager = Locator.GetInstance<IPartnerManager>();

            var partner = this.CreateTestPartner();
            partnerManager.Create(partner);

            var transaction = this.CreateTestAchTransaction();
            transaction.Partner = partner;
            transaction.Locked = true;
            var instance = manager.Create(transaction);

            var transaction2 = this.CreateTestAchTransaction();
            transaction2.Partner = partner;
            transaction2.Locked = true;
            var instance2 = manager.Create(transaction2);

            this.ClearSession(instance);
            this.ClearSession(instance2);
            var trnsList = new List<AchTransactionEntity> { transaction, transaction2 };
            manager.UnLock(trnsList);

            transaction = manager.Load(transaction.Id);
            Assert.IsFalse(transaction.Locked);

            transaction2 = manager.Load(transaction2.Id);
            Assert.IsFalse(transaction2.Locked);
        }*/

        #endregion

        #region Private Methods

        private AchTransactionEntity CreateAchTransaction(
            string callbackUrl = null,
            string dfiAccountId = null,
            string entryClassCode = null,
            string entryDescription = null,
            string individualIdNumber = null,
            string paymentRelatedInfo = null,
            string receiverName = null,
            string serviceClassCode = null,
            string transactionCode = null,
            string transitRoutingNumber = null)
        {
            var defaultPartner = new PersistenceSpecification<PartnerEntity>(Session)
                .CheckProperty(c => c.Name, ShortStringGenerator.GetRandomValue())
                .VerifyTheMappings();

            var transaction = new AchTransactionEntity
                                  {
                                      DfiAccountId = dfiAccountId ?? "12345678901234567",
                                      CallbackUrl = callbackUrl ?? "http://test.com",
                                      EntryDescription = entryDescription ?? "PAYROLL",
                                      IndividualIdNumber = individualIdNumber ?? "123456789012345",
                                      ReceiverName = receiverName ?? "SomeName",
                                      TransitRoutingNumber = transitRoutingNumber ?? "123456789",
                                      EntryClassCode = entryClassCode ?? "PPD",
                                      ServiceClassCode = serviceClassCode ?? "200",
                                      TransactionCode = transactionCode ?? "22",
                                      PaymentRelatedInfo = paymentRelatedInfo ?? "dsdfdsfsdf",
                                      Partner = defaultPartner,
                                      Amount = (decimal)123.00,
                                      Status = AchTransactionStatus.Created,
                                      EntryDate = DateTime.Now
                                  };
            var manager = Locator.GetInstance<IAchTransactionManager>();
            manager.Create(transaction);

            return transaction;
        }

        #endregion
    }
}