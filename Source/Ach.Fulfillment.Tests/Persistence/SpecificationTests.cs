namespace Ach.Fulfillment.Tests.Persistence
{
    using Ach.Fulfillment.Data;

    using FluentNHibernate.Testing;

    using NUnit.Framework;

    [TestFixture]
    public class SpecificationTests : PersistenseTestBase
    {
        #region Public Methods and Operators

        [Test]
        public void PartnerSpecification()
        {
            new PersistenceSpecification<PartnerEntity>(this.Session)
                .CheckProperty(c => c.Name, this.ShortStringGenerator.GetRandomValue())
            .VerifyTheMappings();
        }

        [Test]
        public void PermissionSpecification()
        {
            new PersistenceSpecification<PermissionEntity>(this.Session)
                .CheckProperty(c => c.Name, AccessRight.Authenticate)
            .VerifyTheMappings();
        }

        [Test]
        public void RoleSpecification()
        {
            new PersistenceSpecification<RoleEntity>(this.Session)
                .CheckProperty(c => c.Name, this.ShortStringGenerator.GetRandomValue())
            .VerifyTheMappings();
        }

        [Test]
        public void UserSpecification()
        {
            var role = new PersistenceSpecification<RoleEntity>(this.Session)
                .CheckProperty(c => c.Name, this.ShortStringGenerator.GetRandomValue())
            .VerifyTheMappings();

            new PersistenceSpecification<UserEntity>(this.Session)
                .CheckProperty(c => c.Name, this.ShortStringGenerator.GetRandomValue())
                .CheckProperty(c => c.Email, this.ShortStringGenerator.GetRandomValue())
                .CheckEntity(c => c.Role, role)
            .VerifyTheMappings();
        }

        [Test]
        public void UserPasswordCredentialSpecification()
        {
            var role = new PersistenceSpecification<RoleEntity>(this.Session)
                .CheckProperty(c => c.Name, this.ShortStringGenerator.GetRandomValue())
            .VerifyTheMappings();

            var user = new PersistenceSpecification<UserEntity>(this.Session)
                .CheckProperty(c => c.Name, this.ShortStringGenerator.GetRandomValue())
                .CheckProperty(c => c.Email, this.ShortStringGenerator.GetRandomValue())
                .CheckEntity(c => c.Role, role)
            .VerifyTheMappings();

            var credential = new PersistenceSpecification<UserPasswordCredentialEntity>(this.Session)
                .CheckProperty(c => c.Login, this.ShortStringGenerator.GetRandomValue())
                .CheckProperty(c => c.PasswordHash, this.ShortStringGenerator.GetRandomValue())
                .CheckProperty(c => c.PasswordSalt, this.ShortStringGenerator.GetRandomValue())
                .CheckEntity(c => c.User, user)
            .VerifyTheMappings();

            Assert.That(credential.User.UserPasswordCredentials.Count, Is.EqualTo(1));
        }

        [Test]
        public void PartnerUserSpecification()
        {
            var role = new PersistenceSpecification<RoleEntity>(this.Session)
                .CheckProperty(c => c.Name, this.ShortStringGenerator.GetRandomValue())
            .VerifyTheMappings();

            var user = new PersistenceSpecification<UserEntity>(this.Session)
                .CheckProperty(c => c.Name, this.ShortStringGenerator.GetRandomValue())
                .CheckProperty(c => c.Email, this.ShortStringGenerator.GetRandomValue())
                .CheckEntity(c => c.Role, role)
            .VerifyTheMappings();

            var partner = new PersistenceSpecification<PartnerEntity>(this.Session)
                .CheckProperty(c => c.Name, this.ShortStringGenerator.GetRandomValue())
            .VerifyTheMappings();

            partner.Users.Add(user);

            this.Session.Flush();

            Assert.That(user.Partners.Count, Is.EqualTo(1));
        }

        [Test]
        public void PermissionRoleSpecification()
        {
             var permission = new PersistenceSpecification<PermissionEntity>(this.Session)
                .CheckProperty(c => c.Name, AccessRight.Authenticate)
            .VerifyTheMappings();
        
            var role = new PersistenceSpecification<RoleEntity>(this.Session)
                .CheckProperty(c => c.Name, this.ShortStringGenerator.GetRandomValue())
            .VerifyTheMappings();

            role.Permissions.Add(permission);
            
            this.Session.Flush();

            this.Session.Evict(role);

            role = this.Session.Load<RoleEntity>(role.Id);

            Assert.That(role.Permissions.Count, Is.EqualTo(1));
        }

        #endregion
    }
}