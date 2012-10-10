using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Ach.Fulfillment.Business;
using Ach.Fulfillment.Data;
using Rhino.Mocks;

namespace Ach.Fulfillment.Tests.Business
{
    [TestFixture]
    public class AchTransactionManagerTests : BusinessIntegrationTestBase
    {
        #region Public Methods and Operators
        [Ignore]
        [Test]
        public void CreateAchTransactionTest()
        {
            var manager = this.Locator.GetInstance<IAchTransactionManager>();
            
            var transaction = this.CreateTestTransaction();

            var instance = manager.Create(transaction);

            Assert.That(instance.Id, Is.GreaterThan(0));         
        }
        [Ignore]
        [Test]
        public void GenerateAchFileTest()
        {
            var manager = Locator.GetInstance<IAchTransactionManager>();
            manager.Generate();
        }

        [Ignore]
        [Test]
        public void RemoveTransactionFromQueueTest()
        {
            var manager = this.Locator.GetInstance<IAchTransactionManager>();
            var transaction = this.CreateTestTransaction();
            var instance = manager.Create(transaction);

            var transactions = new List<AchTransactionEntity>{ transaction };
            manager.RemoveTransactionFromQueue(transactions);

            var changedTransaction = manager.Load(instance.Id);
            Assert.AreEqual(changedTransaction.TransactionStatus,TransactionStatus.Batched);
        }

        [Ignore]
        [Test]
        public void CreateFileForPartnerTransactionsTest()
        {
            var mocks = new MockRepository();
            var container = mocks.StrictMock<IUnityContainer>();
            var fileManager = mocks.StrictMock<IFileManager>();
            container.RegisterInstance(fileManager);
            container.RegisterType<IAchTransactionManager>();
            var manager = container.Resolve<IAchTransactionManager>();
            
            var fileEntity = new FileEntity();
            var createFileWasCalled = false;
            var transaction = this.CreateTestTransaction();
            transaction.Partner.Id = 1;
            var trList = new List<AchTransactionEntity> { transaction };

            Expect.Call(fileManager.Create(fileEntity)).Return(fileEntity).WhenCalled(delegate{ createFileWasCalled = true;});
            mocks.ReplayAll();
            
            manager.CreateFileForPartnerTransactions(transaction.Partner,trList,"achfile");

            Assert.IsTrue(createFileWasCalled);
        }

        #endregion
    }
}
