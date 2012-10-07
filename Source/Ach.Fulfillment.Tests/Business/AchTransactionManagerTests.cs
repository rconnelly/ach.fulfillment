using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Ach.Fulfillment.Business;
using Ach.Fulfillment.Data;  

namespace Ach.Fulfillment.Tests.Business
{
    [TestFixture]
    public class AchTransactionManagerTests : BusinessIntegrationTestBase
    {
        #region Public Methods and Operators

        [Test]
        public void CreateAchTransactionTest()
        {
            var manager = this.Locator.GetInstance<IAchTransactionManager>();
            
            var transaction = this.CreateTestTransaction();

            var instance = manager.Create(transaction);

            Assert.That(instance.Id, Is.GreaterThan(0));         
        }

        [Test]
        public void GenerateAchFileTest()
        {
            var manager = this.Locator.GetInstance<IAchTransactionManager>();
            var achfile = manager.Generate();
        }


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

        #endregion
    }
}
