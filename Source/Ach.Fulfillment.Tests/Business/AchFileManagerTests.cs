namespace Ach.Fulfillment.Tests.Business
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using Ach.Fulfillment.Business;
    using Ach.Fulfillment.Business.Impl;
    using Ach.Fulfillment.Common.Exceptions;
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications;
    using Ach.Fulfillment.Data.Specifications.AchFiles;

    using NUnit.Framework;

    [TestFixture]
    public class AchFileManagerTests : BusinessIntegrationTestBase
    {
        #region Public Methods and Operators

        [Test]
        public void CreateTest()
        {
            var manager = Locator.GetInstance<AchFileManager>();
            var transactionManager = Locator.GetInstance<IAchTransactionManager>();
            var partnerManager = Locator.GetInstance<IPartnerManager>();

            var partner = this.CreateTestPartner();
            partnerManager.Create(partner);

            var transaction = this.CreateTestAchTransaction();
            transaction.Partner = partner;
            transactionManager.Create(transaction);

            var instance = manager.Create(partner, transaction);

            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.Id, Is.GreaterThan(0));   
            Assert.NotNull(instance.Transactions);
            Assert.AreEqual(1, instance.Transactions.Count);
            Assert.AreEqual(transaction, instance.Transactions[0]);
            Assert.AreEqual(partner, instance.Partner);
        }

        [Test]
        public void CleanUpCompletedFilesTest()
        {
            var manager = Locator.GetInstance<AchFileManager>();
            var transactionManager = Locator.GetInstance<IAchTransactionManager>();
            var partnerManager = Locator.GetInstance<IPartnerManager>();

            var partner = this.CreateTestPartner();
            partnerManager.Create(partner);

            var transaction = this.CreateTestAchTransaction();
            transaction.Partner = partner;
            transactionManager.Create(transaction);

            var achFile = manager.Create(partner, transaction);

            manager.UpdateStatus(achFile, AchFileStatus.Finalized);
            Assert.AreEqual(AchFileStatus.Finalized, achFile.FileStatus);
            Assert.AreEqual(AchTransactionStatus.Accepted, achFile.Transactions.Single().Status);

            manager.Cleanup();

            var ex = Assert.Throws<ObjectNotFoundException>(() => manager.Load(achFile.Id));
            Trace.WriteLine(ex.Message);

            ex = Assert.Throws<ObjectNotFoundException>(() => transactionManager.Load(transaction.Id));
            Trace.WriteLine(ex.Message);
        }

        [Test]
        public void ChangeAchFilesStatusToUploadedTest()
        {
            var manager = Locator.GetInstance<AchFileManager>();
            var transactionManager = Locator.GetInstance<IAchTransactionManager>();
            var partnerManager = Locator.GetInstance<IPartnerManager>();

            var partner = this.CreateTestPartner();
            partnerManager.Create(partner);

            var transaction = this.CreateTestAchTransaction();
            transaction.Partner = partner;
            transactionManager.Create(transaction);

            var instance = manager.Create(partner, transaction);
            var achFile = instance;

            manager.UpdateStatus(achFile, AchFileStatus.Uploaded);

            this.ClearSession(instance);

            instance = manager.Load(achFile.Id);
            Assert.AreEqual(AchFileStatus.Uploaded, instance.FileStatus);
            Assert.AreEqual(AchTransactionStatus.Sent, instance.Transactions[0].Status);
        }

        [Test]
        public void ChangeAchFilesStatusToCompletedTest()
        {
            var manager = Locator.GetInstance<AchFileManager>();
            var transactionManager = Locator.GetInstance<IAchTransactionManager>();
            var partnerManager = Locator.GetInstance<IPartnerManager>();

            var partner = this.CreateTestPartner();
            partnerManager.Create(partner);

            var transaction = this.CreateTestAchTransaction();
            transaction.Partner = partner;
            transactionManager.Create(transaction);

            var instance = manager.Create(partner, transaction);
            var achFile = instance;

            manager.UpdateStatus(achFile, AchFileStatus.Finalized);

            this.ClearSession(instance);

            instance = manager.Load(achFile.Id);
            Assert.AreEqual(AchFileStatus.Finalized, instance.FileStatus);
            Assert.AreEqual(AchTransactionStatus.Accepted, instance.Transactions[0].Status);
        }

        [Test]
        public void ChangeAchFilesStatusToRejectedTest()
        {
            var manager = Locator.GetInstance<AchFileManager>();
            var transactionManager = Locator.GetInstance<IAchTransactionManager>();
            var partnerManager = Locator.GetInstance<IPartnerManager>();

            var partner = this.CreateTestPartner();
            partnerManager.Create(partner);

            var transaction = this.CreateTestAchTransaction();
            transaction.Partner = partner;
            transactionManager.Create(transaction);

            var instance = manager.Create(partner, transaction);
            var achFile = instance;

            manager.UpdateStatus(achFile, AchFileStatus.Rejected);

            this.ClearSession(instance);

            instance = manager.Load(achFile.Id);
            Assert.AreEqual(AchFileStatus.Rejected, instance.FileStatus);
            Assert.AreEqual(AchTransactionStatus.Rejected, instance.Transactions[0].Status);
        }

        [Test]
        public void GetNextIdModifierTest()
        {
            var manager = Locator.GetInstance<AchFileManager>();
            var transactionManager = Locator.GetInstance<IAchTransactionManager>();
            var partnerManager = Locator.GetInstance<IPartnerManager>();

            var partner = this.CreateTestPartner();
            partnerManager.Create(partner);

            var transaction = this.CreateTestAchTransaction();
            transaction.Partner = partner;
            transactionManager.Create(transaction);

            var instance = manager.Create(partner, transaction);

            this.ClearSession(instance);

            var idmodifier = manager.GetNextIdModifier(partner);
            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.Id, Is.GreaterThan(0));
            Assert.AreEqual("B", idmodifier);
        }

        [Test]
        public void GetNextIdModifierWillReturnAAfterZTest()
        {
            var manager = Locator.GetInstance<AchFileManager>();
            var transactionManager = Locator.GetInstance<IAchTransactionManager>();
            var partnerManager = Locator.GetInstance<IPartnerManager>();

            var partner = this.CreateTestPartner();
            partnerManager.Create(partner);

            var transaction = this.CreateTestAchTransaction();
            transaction.Partner = partner;
            transactionManager.Create(transaction);

            var achFile = this.CreateTestAchFile();
            achFile.FileIdModifier = "Z";
            achFile.Partner = partner;
            achFile.Transactions.Add(transaction);

            var instance = manager.Create(achFile);

            this.ClearSession(instance);

            var idmodifier = manager.GetNextIdModifier(partner);
            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.Id, Is.GreaterThan(0));
            Assert.AreEqual("A", idmodifier);
        }

        [Test]
        public void GenerateAchFileTest()
        {
            var manager = Locator.GetInstance<AchFileManager>();
            var transactionManager = Locator.GetInstance<IAchTransactionManager>();
            var partnerManager = Locator.GetInstance<IPartnerManager>();

            var partner = this.CreateTestPartner();
            partnerManager.Create(partner);

            var transaction = this.CreateTestAchTransaction();
            transaction.Partner = partner;
            transactionManager.Create(transaction);

            manager.ProcessReadyToBeGroupedAchTransactions();

            var achFilesCreated = manager.FindAll(new AchFileForPartner(partner)).ToList();

            Assert.IsNotNull(achFilesCreated);
            Assert.AreEqual(1, achFilesCreated.Count);
            Assert.IsNotNull(achFilesCreated[0].Transactions);
            Assert.AreEqual(transaction, achFilesCreated[0].Transactions[0]);
            Assert.AreEqual(AchFileStatus.Created, achFilesCreated[0].FileStatus);
            Assert.AreEqual(AchTransactionStatus.Batched, achFilesCreated[0].Transactions[0].Status);
        }

        [Test]
        public void GenerateWontCreateAchFileForTransactionSetMoreThenOnceTest()
        {
            var manager = Locator.GetInstance<AchFileManager>();
            var transactionManager = Locator.GetInstance<IAchTransactionManager>();
            var partnerManager = Locator.GetInstance<IPartnerManager>();

            var partner = this.CreateTestPartner();
            partnerManager.Create(partner);

            var transaction = this.CreateTestAchTransaction();
            transaction.Partner = partner;
            transactionManager.Create(transaction);

            manager.ProcessReadyToBeGroupedAchTransactions();

            var achFilesCreated = manager.FindAll(new AchFileForPartner(partner)).ToList();

            Assert.IsNotNull(achFilesCreated);
            Assert.AreEqual(1, achFilesCreated.Count);
            Assert.IsNotNull(achFilesCreated[0].Transactions);
            Assert.AreEqual(transaction, achFilesCreated[0].Transactions[0]);

            manager.ProcessReadyToBeGroupedAchTransactions();

            this.ClearSession();

            achFilesCreated = manager.FindAll(new AchFileForPartner(partner)).ToList();

            Assert.IsNotNull(achFilesCreated);
            Assert.AreEqual(1, achFilesCreated.Count);
            Assert.IsNotNull(achFilesCreated[0].Transactions);
            Assert.AreEqual(transaction, achFilesCreated[0].Transactions[0]);
        }

        /*[Test]
        public void GenerateAchFileWillUnlockTransactionsTest()
        {
            var manager = Locator.GetInstance<AchFileManager>();
            var transactionManager = Locator.GetInstance<IAchTransactionManager>();
            var partnerManager = Locator.GetInstance<IPartnerManager>();

            var partner = this.CreateTestPartner();
            partnerManager.Create(partner);

            var transaction = this.CreateTestAchTransaction();
            transaction.Partner = partner;
            transactionManager.Create(transaction);

            manager.Generate();

            var achFilesCreated = manager.FindAll(new AchFileForPartner(partner)).ToList();

            Assert.IsNotNull(achFilesCreated);
            Assert.AreEqual(1, achFilesCreated.Count);
            Assert.IsNotNull(achFilesCreated[0].Transactions);
            Assert.AreEqual(transaction, achFilesCreated[0].Transactions[0]);
            Assert.IsFalse(achFilesCreated[0].Transactions[0].Locked);
        }*/

        /*[Test]
        public void GetAchFilesDataForUploadingTest()
        {
            var manager = Locator.GetInstance<AchFileManager>();
            var transactionManager = Locator.GetInstance<IAchTransactionManager>();
            var partnerManager = Locator.GetInstance<IPartnerManager>();

            var partner = this.CreateTestPartnerWithDetails();
            partnerManager.Create(partner);

            var transaction = this.CreateTestAchTransaction();
            transaction.Partner = partner;
            transactionManager.Create(transaction);

            manager.Generate();

            var achFiles = manager.GetAchFilesForUploading().ToList();

            Assert.IsNotNull(achFiles);
            Assert.GreaterOrEqual(achFiles.Count(), 1);
        }*/


        #endregion
    }
}
