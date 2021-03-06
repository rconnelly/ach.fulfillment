namespace Ach.Fulfillment.Tests.Business
{
    using Ach.Fulfillment.Common;
    using Ach.Fulfillment.Common.Transactions;
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Tests.Common;

    using Microsoft.Practices.ServiceLocation;

    using NHibernate;

    using NUnit.Framework;

    using QuickGenerate.Primitives;

    public abstract class BusinessIntegrationTestBase : TestBase
    {
        #region Constants and Fields

        protected readonly StringGenerator ShortStringGenerator = new StringGenerator(5, MetadataInfo.StringShort);

        protected readonly StringGenerator NormalStringGenerator = new StringGenerator(5, MetadataInfo.StringNormal);

        private UnitOfWork unitOfWork;

        #endregion

        #region Public Properties

        public ISession Session { get; set; }

        public IServiceLocator Locator 
        { 
            get
            {
                return ServiceLocator.Current;
            } 
        }

        protected Transaction Transaction { get; private set; }

        #endregion

        #region Public Methods and Operators

        [SetUp]
        public virtual void SetUp()
        {
            this.unitOfWork = new UnitOfWork();
            this.Transaction = new Transaction();
            this.Session = ServiceLocator.Current.GetInstance<ISession>();
        }

        [TearDown]
        public virtual void TearDown()
        {
            this.Transaction.Dispose();
            this.unitOfWork.Dispose();
        }

        protected void ClearSession(params object[] instances)
        {
            var session = this.Locator.GetInstance<ISession>();
            session.Flush();
            foreach (var instance in instances)
            {
                session.Evict(instance);
            }
        }

        #endregion
    }
}