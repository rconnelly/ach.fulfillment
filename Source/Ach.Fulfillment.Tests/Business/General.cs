namespace Ach.Fulfillment.Tests.Business
{
    using System;

    using Ach.Fulfillment.Business;
    using Ach.Fulfillment.Common;
    using Ach.Fulfillment.Data;
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

        public INotificationManager NotificationManager
        {
            get
            {
                return this.Locator.GetInstance<INotificationManager>();
            }
        }

        [Test]
        public void Default()
        {
            /*using (new UnitOfWork())
            {
                this.NotificationManager.RaiseAchFileStatusChangedNotification(new AchFileEntity { Id = 2, FileStatus = AchFileStatus.Created });

                AchFileEntity file;
                this.NotificationManager.TryGetNextReadyToGenerateAchFile(out file);
                this.NotificationManager.DeliverRemoteNotifications();
            }

            return;
*/
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
                while (this.NotificationManager.DeliverRemoteNotifications())
                {
                }
            }

            // - clean old data
            using (new UnitOfWork())
            {
                this.AchFileManager.Cleanup();
            }
        }
    }
}
