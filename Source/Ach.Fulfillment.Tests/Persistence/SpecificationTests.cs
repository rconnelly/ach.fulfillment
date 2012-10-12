using System;
using QuickGenerate.Primitives;

namespace Ach.Fulfillment.Tests.Persistence
{
    using Data;

    using FluentNHibernate.Testing;

    using NUnit.Framework;

    [TestFixture]
    public class SpecificationTests : PersistenseTestBase
    {
        #region Public Methods and Operators

        [Test]
        public void PartnerSpecification()
        {
            new PersistenceSpecification<PartnerEntity>(Session)
                .CheckProperty(c => c.Name, ShortStringGenerator.GetRandomValue())
            .VerifyTheMappings();
        }

        [Test]
        public void PermissionSpecification()
        {
            var role = new PersistenceSpecification<RoleEntity>(Session)
                .CheckProperty(c => c.Name, ShortStringGenerator.GetRandomValue())
            .VerifyTheMappings();

            new PersistenceSpecification<PermissionEntity>(Session)
                .CheckProperty(c => c.Name, AccessRight.Admin)
                .CheckEntity(c => c.Role, role)
            .VerifyTheMappings();
        }

        [Test]
        public void RoleSpecification()
        {
            new PersistenceSpecification<RoleEntity>(Session)
                .CheckProperty(c => c.Name, ShortStringGenerator.GetRandomValue())
            .VerifyTheMappings();
        }

        [Test]
        public void UserSpecification()
        {
            var role = new PersistenceSpecification<RoleEntity>(Session)
                .CheckProperty(c => c.Name, ShortStringGenerator.GetRandomValue())
            .VerifyTheMappings();

            new PersistenceSpecification<UserEntity>(Session)
                .CheckProperty(c => c.Name, ShortStringGenerator.GetRandomValue())
                .CheckProperty(c => c.Email, ShortStringGenerator.GetRandomValue())
                .CheckEntity(c => c.Role, role)
            .VerifyTheMappings();
        }

        [Test]
        public void UserPasswordCredentialSpecification()
        {
            var role = new PersistenceSpecification<RoleEntity>(Session)
                .CheckProperty(c => c.Name, ShortStringGenerator.GetRandomValue())
            .VerifyTheMappings();

            var user = new PersistenceSpecification<UserEntity>(Session)
                .CheckProperty(c => c.Name, ShortStringGenerator.GetRandomValue())
                .CheckProperty(c => c.Email, ShortStringGenerator.GetRandomValue())
                .CheckEntity(c => c.Role, role)
            .VerifyTheMappings();

            var credential = new PersistenceSpecification<UserPasswordCredentialEntity>(Session)
                .CheckProperty(c => c.Login, ShortStringGenerator.GetRandomValue())
                .CheckProperty(c => c.PasswordHash, ShortStringGenerator.GetRandomValue())
                .CheckProperty(c => c.PasswordSalt, ShortStringGenerator.GetRandomValue())
                .CheckEntity(c => c.User, user)
            .VerifyTheMappings();

            Assert.That(credential.User.UserPasswordCredential, Is.Not.Null);
        }

        [Test]
        public void PartnerUserSpecification()
        {
            var role = new PersistenceSpecification<RoleEntity>(Session)
                .CheckProperty(c => c.Name, ShortStringGenerator.GetRandomValue())
            .VerifyTheMappings();

            var user = new PersistenceSpecification<UserEntity>(Session)
                .CheckProperty(c => c.Name, ShortStringGenerator.GetRandomValue())
                .CheckProperty(c => c.Email, ShortStringGenerator.GetRandomValue())
                .CheckEntity(c => c.Role, role)
            .VerifyTheMappings();

            var partner = new PersistenceSpecification<PartnerEntity>(Session)
                .CheckProperty(c => c.Name, ShortStringGenerator.GetRandomValue())
            .VerifyTheMappings();

            partner.Users.Add(user);

            Session.Flush();
            Session.Clear();

            user = Session.Load<UserEntity>(user.Id);

            Assert.That(user.Partner, Is.Not.Null);
        }

        [Test]
        public void PermissionRoleSpecification()
        {
            var role = new PersistenceSpecification<RoleEntity>(Session)
                .CheckProperty(c => c.Name, ShortStringGenerator.GetRandomValue())
            .VerifyTheMappings();

             var permission = new PersistenceSpecification<PermissionEntity>(Session)
                .CheckProperty(c => c.Name, AccessRight.Admin)
                .CheckEntity(c => c.Role, role)
            .VerifyTheMappings();
        
            Session.Evict(permission);
            Session.Evict(role);

            role = Session.Load<RoleEntity>(role.Id);

            Assert.That(role.Permissions.Count, Is.EqualTo(1));
        }

        [Test]
        public void AchTransactionSpecification()
        {
            var partner = new PersistenceSpecification<PartnerEntity>(Session)
                .CheckProperty(c => c.Name, ShortStringGenerator.GetRandomValue())
            .VerifyTheMappings();

            new PersistenceSpecification<PartnerDetailEntity>(Session)
                .CheckProperty(c => c.CompanyIdentification, new StringGenerator(10, 10).GetRandomValue())
                .CheckProperty(c => c.CompanyName, new StringGenerator(5, 16).GetRandomValue())
                .CheckProperty(c => c.DfiIdentification, new StringGenerator(8, 8).GetRandomValue())
                .CheckProperty(c => c.Destination, new StringGenerator(5, 23).GetRandomValue())
                .CheckProperty(c => c.DiscretionaryData, new StringGenerator(5, 20).GetRandomValue())
                .CheckProperty(c => c.ImmediateDestination, new StringGenerator(5, 10).GetRandomValue())
                .CheckProperty(c => c.OriginOrCompanyName, new StringGenerator(5, 23).GetRandomValue())
                .CheckEntity(c => c.Partner, partner)
            .VerifyTheMappings();

            new PersistenceSpecification<AchTransactionEntity>(Session)
                .CheckProperty(c => c.IndividualIdNumber, new StringGenerator(15, 15).GetRandomValue())
                .CheckProperty(c => c.ReceiverName, new StringGenerator(22, 22).GetRandomValue())
                .CheckProperty(c => c.Amount, (decimal)123.45)
                .CheckProperty(c => c.CallbackUrl, new StringGenerator(20, 255).GetRandomValue())
                .CheckProperty(c => c.DfiAccountId, new StringGenerator(17, 17).GetRandomValue())
                .CheckProperty(c => c.EntryClassCode, new StringGenerator(3, 3).GetRandomValue())
                .CheckProperty(c => c.EntryDate, DateTime.Now)
                .CheckProperty(c => c.EntryDescription, new StringGenerator(5, 10).GetRandomValue())
                .CheckProperty(c => c.PaymentRelatedInfo, new StringGenerator(4, 84).GetRandomValue())
                .CheckProperty(c => c.ServiceClassCode, new StringGenerator(3, 3).GetRandomValue())
                .CheckProperty(c => c.TransactionCode, new StringGenerator(2, 2).GetRandomValue())
                .CheckProperty(c => c.TransactionStatus, AchTransactionStatus.Received)
                .CheckProperty(c => c.TransitRoutingNumber, new StringGenerator(9, 9).GetRandomValue())
                .CheckEntity(c => c.Partner, partner)
            .VerifyTheMappings();
        }

        [Test]
        public void AchFileSpecification()
        {
             var partner = new PersistenceSpecification<PartnerEntity>(Session)
                .CheckProperty(c => c.Name, ShortStringGenerator.GetRandomValue())
            .VerifyTheMappings();

            new PersistenceSpecification<PartnerDetailEntity>(Session)
                .CheckProperty(c => c.CompanyIdentification, new StringGenerator(10, 10).GetRandomValue())
                .CheckProperty(c => c.CompanyName, new StringGenerator(5, 16).GetRandomValue())
                .CheckProperty(c => c.DfiIdentification, new StringGenerator(8, 8).GetRandomValue())
                .CheckProperty(c => c.Destination, new StringGenerator(5, 23).GetRandomValue())
                .CheckProperty(c => c.DiscretionaryData, new StringGenerator(5, 20).GetRandomValue())
                .CheckProperty(c => c.ImmediateDestination, new StringGenerator(5, 10).GetRandomValue())
                .CheckProperty(c => c.OriginOrCompanyName, new StringGenerator(5, 23).GetRandomValue())
                .CheckEntity(c => c.Partner, partner)
            .VerifyTheMappings();

            new PersistenceSpecification<FileEntity>(Session)
                .CheckProperty(c=>c.FileIdModifier, "A")
                .CheckProperty(c=>c.FileStatus, 0)
                .CheckProperty(c => c.Locked, false)
                .CheckProperty(c => c.Name, new StringGenerator(16, 16).GetRandomValue())
                .CheckEntity(c => c.Partner, partner)
            .VerifyTheMappings();
        }

        [Test]
        public void FileTransactionSpecification()
        {
            var partner = new PersistenceSpecification<PartnerEntity>(Session)
               .CheckProperty(c => c.Name, ShortStringGenerator.GetRandomValue())
           .VerifyTheMappings();

            new PersistenceSpecification<PartnerDetailEntity>(Session)
                .CheckProperty(c => c.CompanyIdentification, new StringGenerator(10, 10).GetRandomValue())
                .CheckProperty(c => c.CompanyName, new StringGenerator(5, 16).GetRandomValue())
                .CheckProperty(c => c.DfiIdentification, new StringGenerator(8, 8).GetRandomValue())
                .CheckProperty(c => c.Destination, new StringGenerator(5, 23).GetRandomValue())
                .CheckProperty(c => c.DiscretionaryData, new StringGenerator(5, 20).GetRandomValue())
                .CheckProperty(c => c.ImmediateDestination, new StringGenerator(5, 10).GetRandomValue())
                .CheckProperty(c => c.OriginOrCompanyName, new StringGenerator(5, 23).GetRandomValue())
                .CheckEntity(c => c.Partner, partner)
            .VerifyTheMappings();

            var transaction = new PersistenceSpecification<AchTransactionEntity>(Session)
                .CheckProperty(c => c.IndividualIdNumber, new StringGenerator(15, 15).GetRandomValue())
                .CheckProperty(c => c.ReceiverName, new StringGenerator(22, 22).GetRandomValue())
                .CheckProperty(c => c.Amount, (decimal) 123.45)
                .CheckProperty(c => c.CallbackUrl, new StringGenerator(20, 255).GetRandomValue())
                .CheckProperty(c => c.DfiAccountId, new StringGenerator(17, 17).GetRandomValue())
                .CheckProperty(c => c.EntryClassCode, new StringGenerator(3, 3).GetRandomValue())
                .CheckProperty(c => c.EntryDate, DateTime.Now)
                .CheckProperty(c => c.EntryDescription, new StringGenerator(5, 10).GetRandomValue())
                .CheckProperty(c => c.PaymentRelatedInfo, new StringGenerator(4, 84).GetRandomValue())
                .CheckProperty(c => c.ServiceClassCode, new StringGenerator(3, 3).GetRandomValue())
                .CheckProperty(c => c.TransactionCode, new StringGenerator(2, 2).GetRandomValue())
                .CheckProperty(c => c.TransactionStatus, AchTransactionStatus.Received)
                .CheckProperty(c => c.TransitRoutingNumber, new StringGenerator(9, 9).GetRandomValue())
                .CheckEntity(c => c.Partner, partner)
            .VerifyTheMappings();

            var file = new PersistenceSpecification<FileEntity>(Session)
                .CheckProperty(c => c.FileIdModifier, "A")
                .CheckProperty(c => c.FileStatus, 0)
                .CheckProperty(c => c.Locked, false)
                .CheckProperty(c => c.Name, new StringGenerator(16, 16).GetRandomValue())
                .CheckEntity(c => c.Partner, partner)
            .VerifyTheMappings();

            file.Transactions.Add(transaction);

            Session.Flush();
            Session.Clear();

            file = Session.Load<FileEntity>(file.Id);
            transaction = Session.Load<AchTransactionEntity>(transaction.Id);

            Assert.That(file.Transactions, Is.Not.Null);
            Assert.AreEqual(file.Transactions[0], transaction);
        }

        [Test]
        public void PartnerDetailsSpecification()
        {
            var partner = new PersistenceSpecification<PartnerEntity>(Session)
                .CheckProperty(c => c.Name, ShortStringGenerator.GetRandomValue())
            .VerifyTheMappings();

            new PersistenceSpecification<PartnerDetailEntity>(Session)
                .CheckProperty(c => c.CompanyIdentification, new StringGenerator(10, 10).GetRandomValue())
                .CheckProperty(c => c.CompanyName, new StringGenerator(5, 16).GetRandomValue())
                .CheckProperty(c => c.DfiIdentification, new StringGenerator(8, 8).GetRandomValue())
                .CheckProperty(c => c.Destination, new StringGenerator(5, 23).GetRandomValue())
                .CheckProperty(c => c.DiscretionaryData, new StringGenerator(5, 20).GetRandomValue())
                .CheckProperty(c => c.ImmediateDestination, new StringGenerator(5, 10).GetRandomValue())
                .CheckProperty(c => c.OriginOrCompanyName, new StringGenerator(5, 23).GetRandomValue())
                .CheckEntity(c=>c.Partner,partner)
                .VerifyTheMappings();

            //var partner = new PartnerEntity{ Name = "Test"};
            //partner.Details = new PartnerDetailEntity
            //                      {
            //                          CompanyIdentification = new StringGenerator(10, 10).GetRandomValue(),
            //                          CompanyName = new StringGenerator(5, 16).GetRandomValue(),
            //                          DfiIdentification = new StringGenerator(8, 8).GetRandomValue(),
            //                          Destination = new StringGenerator(5, 23).GetRandomValue(),
            //                          DiscretionaryData = new StringGenerator(5, 20).GetRandomValue(),
            //                          ImmediateDestination = new StringGenerator(5, 10).GetRandomValue(),
            //                          OriginOrCompanyName = new StringGenerator(5, 23).GetRandomValue(),
            //                          Partner = partner
            //                      };
            //Session.Save(partner);

            Session.Evict(partner);

            partner = Session.Load<PartnerEntity>(partner.Id);

            Assert.That(partner.Details, Is.Not.Null);


        }

        #endregion
    }
}