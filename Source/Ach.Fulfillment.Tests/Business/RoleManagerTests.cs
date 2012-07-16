namespace Ach.Fulfillment.Tests.Business
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using Ach.Fulfillment.Business;
    using Ach.Fulfillment.Business.Exceptions;
    using Ach.Fulfillment.Common.Exceptions;
    using Ach.Fulfillment.Data;

    using NUnit.Framework;

    [TestFixture]
    public class RoleManagerTests : BusinessIntegrationTestBase
    {
        #region Constants and Fields

        #endregion

        #region Public Methods and Operators

        [Test]
        public void Create()
        {
            var manager = this.Locator.GetInstance<IRoleManager>();

            var role = this.CreateTestRole();

            var instance = manager.Create(role);

            Assert.That(instance.Id, Is.GreaterThan(0));
            Assert.That(instance.Permissions, Is.Not.Null);
            Assert.That(instance.Permissions.Count, Is.EqualTo(1));
        }

        [Test]
        public void Delete()
        {
            var manager = this.Locator.GetInstance<IRoleManager>();
            var instance = manager.Create(this.CreateTestRole());
            this.Session.Flush();
            manager.Delete(instance);
        }

        [Test]
        public void FindAll()
        {
            var manager = this.Locator.GetInstance<IRoleManager>();

            manager.Create(this.CreateTestRole());

            var roles = manager.FindAll();
            Assert.That(roles, Is.Not.Null);
            Assert.That(roles.Count(), Is.GreaterThan(0));
        }

        [Test]
        public void Load()
        {
            var manager = this.Locator.GetInstance<IRoleManager>();

            var instance = manager.Create(this.CreateTestRole());
            Assert.That(instance, Is.Not.Null);

            var role = manager.Load(instance.Id);
            Assert.That(role, Is.Not.Null);

            Assert.That(role.Name, Is.EquivalentTo(instance.Name));
        }

        [Test]
        public void LoadByName()
        {
            var manager = this.Locator.GetInstance<IRoleManager>();

            var instance = manager.Create(this.CreateTestRole());
            Assert.That(instance, Is.Not.Null);

            var role = manager.Load(instance.Name);
            Assert.That(role, Is.Not.Null);
            Assert.That(role.Name, Is.EquivalentTo(instance.Name));
            Assert.That(role.Id, Is.EqualTo(instance.Id));

            Assert.Throws<ObjectNotFoundException>(() => manager.Load(this.ShortStringGenerator.GetRandomValue()));
        }

        [Test]
        public void NotUniqueRole()
        {
            var manager = this.Locator.GetInstance<IRoleManager>();
            var instance = manager.Create(this.CreateTestRole());

            var ex =
                Assert.Throws<BusinessValidationException>(() => manager.Create(new RoleEntity { Name = instance.Name }));
            Trace.WriteLine(ex.Message);
        }

        [Test]
        public void Update()
        {
            var manager = this.Locator.GetInstance<IRoleManager>();
            var role = this.CreateTestRole();
            var instance = manager.Create(role);

            instance.Name = this.ShortStringGenerator.GetRandomValue();

            manager.Update(instance);

            this.Session.Flush();
        }

        #endregion

        #region Methods

        private RoleEntity CreateTestRole()
        {
            var role = new RoleEntity
                {
                    Name = this.ShortStringGenerator.GetRandomValue(), 
                    Permissions = new List<PermissionEntity> { new PermissionEntity { Name = AccessRight.Authenticate } }
                };
            return role;
        }

        #endregion

        /*[Test]
        public void IllegalDelete()
        {
            var manager = this.Locator.GetInstance<ICurrencyManager>();
            var currency = manager.Load(1);
            Assert.Throws<DeleteConstraintException>(() => manager.Delete(currency));
        }*/
    }
}