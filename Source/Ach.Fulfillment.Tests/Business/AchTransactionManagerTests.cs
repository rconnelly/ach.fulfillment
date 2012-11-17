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
            var transaction = this.CreateTestTransaction();
            transaction.Partner = partner;
            
            var instance = manager.Create(transaction);
            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.Id, Is.GreaterThan(0));         
        }

        [Test]
        public void LoadTest()
        {
            var manager = Locator.GetInstance<IAchTransactionManager>();
            var partnerManager = Locator.GetInstance<IPartnerManager>();

            var partner = this.CreateTestPartner();
            partnerManager.Create(partner);
            var transaction = this.CreateTestTransaction();
            transaction.Partner = partner;
            var instance = manager.Create(transaction);

            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.Id, Is.GreaterThan(0));
           
            this.ClearSession(instance);

            transaction = manager.Load(instance.Id);
            Assert.That(transaction, Is.Not.Null);
        }

        [Test]
        public void ChangeAchTransactionStatusTest()
        {
            var manager = Locator.GetInstance<IAchTransactionManager>();
            var partnerManager = Locator.GetInstance<IPartnerManager>();

            var partner = this.CreateTestPartner();
            partnerManager.Create(partner);

            var transaction = this.CreateTestTransaction();
            transaction.Partner = partner;

            var instance = manager.Create(transaction);

            var transactions = new List<AchTransactionEntity> { transaction };
            manager.ChangeAchTransactionStatus(transactions, AchTransactionStatus.Batched);

            var changedTransaction = manager.Load(instance.Id);
            Assert.AreEqual(changedTransaction.TransactionStatus, AchTransactionStatus.Batched);
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
            Assert.That(ex.Errors.Count(), Is.EqualTo(1));

            Trace.WriteLine("CallbackUrl test");
            ex = Assert.Throws<BusinessValidationException>(() => this.CreateAchTransaction(new string('&', MetadataInfo.StringLong)));
            Trace.WriteLine(ex.Message);
            Assert.That(ex.Errors.Count(), Is.EqualTo(1));

            Trace.WriteLine("DfiAccountId test");
            ex = Assert.Throws<BusinessValidationException>(() => this.CreateAchTransaction(entryClassCode: "QQqq#$&"));
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
            Assert.That(ex.Errors.Count(), Is.EqualTo(2));

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
                                      CallbackUrl = callbackUrl ?? "test.com",
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
                                      TransactionStatus = AchTransactionStatus.Received,
                                      EntryDate = DateTime.Now
                                  };
            var manager = Locator.GetInstance<IAchTransactionManager>();
            manager.Create(transaction);

            return transaction;
        }

        #endregion
    }
}
