namespace Ach.Fulfillment.Tests.Business
{
    using Ach.Fulfillment.Business;
    using Ach.Fulfillment.Data;

    using Microsoft.Practices.Unity;

    using NUnit.Framework;

    using Rhino.Mocks;

    [TestFixture]
    public class AchFileManagerTests : BusinessIntegrationTestBase
    {
        #region Public Methods and Operators

        [Ignore]
        [Test]
        public void GenerateAchFileTest()
        {
            var manager = Locator.GetInstance<IAchTransactionManager>();
            var partnerManager = Locator.GetInstance<IPartnerManager>();

            var partner = this.CreateTestPartner();
            partnerManager.Create(partner);

            var transaction = this.CreateTestTransaction();
            transaction.Partner = partner;

            manager.Create(transaction);

            // manager.Generate();
        }

        [Ignore]
        [Test]
        public void CreateFileForPartnerTransactionsTest()
        {
            var mocks = new MockRepository();
            var container = mocks.StrictMock<IUnityContainer>();
            var fileManager = mocks.DynamicMock<IAchFileManager>();
            container.RegisterInstance(fileManager);
            container.RegisterType<IAchTransactionManager>();

            // var manager = container.Resolve<IAchTransactionManager>();            
            var fileEntity = new AchFileEntity();
            var createFileWasCalled = false;
            var transaction = this.CreateTestTransaction();
            transaction.Partner.Id = 1;

            // var trnList = new List<AchTransactionEntity> { transaction };
            Expect.Call(fileManager.Create(fileEntity)).Return(fileEntity).WhenCalled(
                delegate { createFileWasCalled = true; });
            mocks.ReplayAll();

            // manager.Create(transaction.Partner, trnList, "achfile");
            Assert.IsTrue(createFileWasCalled);
        }
        #endregion
    }
}
