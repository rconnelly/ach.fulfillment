namespace Ach.Fulfillment.Test.Business
{
    using System.Diagnostics;
    using System.Linq;

    using Ach.Fulfillment.Business;
    using Ach.Fulfillment.Business.Exceptions;

    using NHibernate;

    using NUnit.Framework;

    [TestFixture]
    public class UniverseManagerTests : BusinessIntegrationTestBase
    {
        #region Public Methods and Operators

        [Test]
        public void ChangePassword()
        {
            // create new universe
            var manager = this.Locator.GetInstance<IUniverseManager>();
            var login = this.ShortStringGenerator.GetRandomValue();
            var password = this.ShortStringGenerator.GetRandomValue();
            var instance = manager.Create(login, password);

            // clear session
            this.ClearSession(instance);

            // load universe and change password
            instance = manager.Find(login, password);
            var newPassword = this.ShortStringGenerator.GetRandomValue();
            manager.ChangePassword(instance, newPassword);

            // clear session
            this.ClearSession(instance);

            // check new password
            instance = manager.Find(login, newPassword);
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void Create()
        {
            var manager = this.Locator.GetInstance<IUniverseManager>();
            var login = this.ShortStringGenerator.GetRandomValue();
            var password = this.ShortStringGenerator.GetRandomValue();
            var instance = manager.Create(login, password);
            Assert.That(instance.Id, Is.GreaterThan(0));
        }

        [Test]
        public void Delete()
        {
            var manager = this.Locator.GetInstance<IUniverseManager>();

            // create new password
            var instance = manager.Create(
                this.ShortStringGenerator.GetRandomValue(), this.ShortStringGenerator.GetRandomValue());
            Assert.That(instance.Deleted, Is.False);
            manager.Delete(instance);

            // clear session
            this.ClearSession(instance);

            // check deleted
            instance = manager.Load(instance.Id);
            Assert.That(instance.Deleted, Is.True);
        }

        [Test]
        public void DuplicatedLogin()
        {
            var manager = this.Locator.GetInstance<IUniverseManager>();
            var instance = manager.Load(1);
            var login = instance.Login;
            var password = this.ShortStringGenerator.GetRandomValue();
            var ex = Assert.Throws<BusinessValidationException>(() => manager.Create(login, password));
            Trace.WriteLine(ex);
        }

        [Test]
        public void Find()
        {
            // create new
            var manager = this.Locator.GetInstance<IUniverseManager>();
            var login = this.ShortStringGenerator.GetRandomValue();
            var password = this.ShortStringGenerator.GetRandomValue();
            var instance = manager.Create(login, password);

            // clear session
            this.ClearSession(instance);

            // check login
            instance = manager.Find(login, password);
            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.Login, Is.EqualTo(login));
        }

        [Test]
        public void InvalidData()
        {
            var manager = this.Locator.GetInstance<IUniverseManager>();
            var login = new string('a', 256);
            var password = this.ShortStringGenerator.GetRandomValue();
            var ex = Assert.Throws<BusinessValidationException>(() => manager.Create(login, password));
            Trace.WriteLine(ex.Message);
        }

        [Test]
        public void Load()
        {
            var manager = this.Locator.GetInstance<IUniverseManager>();
            const long Id = 1L;
            var instance = manager.Load(Id);
            Assert.That(instance.Id, Is.EqualTo(Id));
        }

        [Test]
        public void LoadAll()
        {
            var manager = this.Locator.GetInstance<IUniverseManager>();
            var instances = manager.LoadAll(false);
            Assert.That(instances.All(i => !i.Deleted), Is.True);

            var instance = manager.Create(
                this.ShortStringGenerator.GetRandomValue(), this.ShortStringGenerator.GetRandomValue());
            manager.Delete(instance);

            this.ClearSession(instance);

            instances = manager.LoadAll(true);
            Assert.That(instances.Any(i => i.Deleted), Is.True);
        }

        #endregion

        #region Methods

        private void ClearSession(params object[] instances)
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