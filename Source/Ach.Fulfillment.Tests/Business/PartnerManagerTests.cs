namespace Ach.Fulfillment.Tests.Business
{
    using System.Linq;

    using Ach.Fulfillment.Business;

    using NUnit.Framework;

    [TestFixture]
    public class PartnerManagerTests : BusinessIntegrationTestBase
    {
        #region Constants and Fields

        #endregion

        #region Public Methods and Operators

        [Test]
        public void Create()
        {
            var manager = this.Locator.GetInstance<IPartnerManager>();

            var partner = this.CreateTestPartner();

            var instance = manager.Create(partner);

            Assert.That(instance.Id, Is.GreaterThan(0));
        }

        [Test]
        public void Delete()
        {
            var manager = this.Locator.GetInstance<IPartnerManager>();
            var instance = manager.Create(this.CreateTestPartner());

            this.ClearSession(instance);

            manager.Delete(instance);
        }

        [Test]
        public void FindAll()
        {
            var manager = this.Locator.GetInstance<IPartnerManager>();

            var instance = manager.Create(this.CreateTestPartner());

            this.ClearSession(instance);

            var partners = manager.FindAll();
            Assert.That(partners, Is.Not.Null);
            Assert.That(partners.Count(), Is.GreaterThan(0));
        }

        [Test]
        public void Load()
        {
            var manager = this.Locator.GetInstance<IPartnerManager>();

            var instance = manager.Create(this.CreateTestPartner());
            Assert.That(instance, Is.Not.Null);

            this.ClearSession(instance);

            var partner = manager.Load(instance.Id);
            Assert.That(partner, Is.Not.Null);

            Assert.That(partner.Name, Is.EquivalentTo(instance.Name));
        }

        [Test]
        public void Update()
        {
            var manager = this.Locator.GetInstance<IPartnerManager>();
            var partner = this.CreateTestPartner();
            var instance = manager.Create(partner);

            Assert.That(instance, Is.Not.Null);
            this.ClearSession(instance);

            instance.Name = this.ShortStringGenerator.GetRandomValue();

            manager.Update(instance);

            this.ClearSession(instance);

            var ni = manager.Load(instance.Id);
            Assert.That(instance.Name, Is.EqualTo(ni.Name));
        }

        [Test]
        public void AddUser()
        {
            var manager = this.Locator.GetInstance<IPartnerManager>();
            var userManager = this.Locator.GetInstance<IUserManager>();
            var partner = this.CreateTestPartner();
            var user = this.CreateTestUser();
            var partnerInstance = manager.Create(partner);
            var userInstance = userManager.Create(user, this.ShortStringGenerator.GetRandomValue());

            Assert.That(partnerInstance, Is.Not.Null);
            Assert.That(userInstance, Is.Not.Null);
            
            manager.AddUser(partnerInstance, userInstance);

            this.ClearSession(partnerInstance, userInstance);

            var p = manager.Load(partner.Id);

            Assert.That(p, Is.Not.Null);
            Assert.That(p.Users, Is.Not.Null);
            Assert.That(p.Users.Count, Is.GreaterThan(0));
        }

        #endregion
    }
}