namespace Ach.Fulfillment.Tests.Business
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    using Ach.Fulfillment.Business;
    using Ach.Fulfillment.Common;
    using Ach.Fulfillment.Common.Transactions;
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications.AchFiles;
    using Ach.Fulfillment.Persistence;
    using Ach.Fulfillment.Tests.Common;

    using Microsoft.Practices.ServiceLocation;

    using NHibernate;

    using NUnit.Framework;

    using Renci.SshNet;

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
        public void Default()
        {
            /*using (new UnitOfWork())
            {
                using (var tr = new Transaction())
                {
                    var partner = EntityHelper.CreateTestPartner(null);
                    this.Repository.Create(partner);
                    var previous =
                        this.Repository.Query(new AchFileForPartner(partner))
                            .Where(m => m.Created.Date == DateTime.UtcNow.Date)
                            .Max(m => m.FileIdModifier);
                    Trace.WriteLine(previous);
                }
            }

            return;*/

            // - obtain ach transaction from client
            using (new UnitOfWork())
            {
                var user = this.UserManager.GetDefaultUser();
                var partner = user.Partner;

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
                var connection = new PasswordConnectionInfo("localhost", 21, "username", "password");
                this.AchFileManager.ProcessReadyToBeUploadedAchFile(connection);
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
