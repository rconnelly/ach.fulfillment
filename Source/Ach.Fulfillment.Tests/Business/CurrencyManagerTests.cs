namespace Ach.Fulfillment.Tests.Business
{
    using System.Diagnostics;
    using System.Linq;

    using Ach.Fulfillment.Business;
    using Ach.Fulfillment.Business.Exceptions;
    using Ach.Fulfillment.Common.Exceptions;
    using Ach.Fulfillment.Data;

    using NUnit.Framework;

    using Rhino.Mocks;

    [TestFixture]
    public class CurrencyManagerTests : BusinessIntegrationTestBase
    {
        private readonly MockRepository mocker = new MockRepository();

        [Test]
        public void Create()
        {
            var universe = this.Locator.GetInstance<IUniverseManager>().Load(1);
            var manager = this.Locator.GetInstance<ICurrencyManager>();

            var data = new CurrencyEntity
                {
                    Name = this.ShortStringGenerator.GetRandomValue(),
                    Description = this.ShortStringGenerator.GetRandomValue(),
                    CurrencyCode = "usd",
                    Hidden = false,
                    Position = 1
                };
            var instance = manager.Create(universe, data);
            Assert.That(instance.Id, Is.GreaterThan(0));
        }

        [Test]
        public void NotUniqueCurrency()
        {
            var original = this.Session.Get<CurrencyEntity>(1L);
            var universe = this.Session.Load<UniverseEntity>(1L);

            var data = this.mocker.Stub<CurrencyEntity>();
            data.Name = original.Name;

            var manager = this.Locator.GetInstance<ICurrencyManager>();
            var ex = Assert.Throws<BusinessValidationException>(() => manager.Create(universe, data));
            Trace.WriteLine(ex.Message);
        }

        [Test]
        public void InvalidCurrency()
        {
            var universe = this.Locator.GetInstance<IUniverseManager>().Load(1);

            var data = this.mocker.Stub<CurrencyEntity>();
            data.Name = null;
            data.Description = new string('a', MetadataInfo.StringLong + 1);
            data.CurrencyCode = new string('a', MetadataInfo.StringTiny + 1);

            var manager = this.Locator.GetInstance<ICurrencyManager>();
            var ex = Assert.Throws<BusinessValidationException>(() => manager.Create(universe, data));
            Trace.WriteLine(ex.Message);
            Assert.That(ex.Errors.Count(), Is.EqualTo(3));

            data.Name = new string('a', MetadataInfo.StringNormal + 1);
            data.CurrencyCode = "йнд";
            ex = Assert.Throws<BusinessValidationException>(() => manager.Create(universe, data));
            Trace.WriteLine(ex.Message);
            Assert.That(ex.Errors.Count(), Is.EqualTo(3));
        }

        [Test]
        public void Load()
        {
            var manager = this.Locator.GetInstance<ICurrencyManager>();
            var currency = manager.Load(1);
            Assert.That(currency, Is.Not.Null);
        }

        [Test]
        public void LoadByName()
        {
            var universe = this.Locator.GetInstance<IUniverseManager>().Load(1);

            var manager = this.Locator.GetInstance<ICurrencyManager>();
            var currency = manager.Load(universe, "UAH");
            Assert.That(currency, Is.Not.Null);

            Assert.Throws<ObjectNotFoundException>(() => manager.Load(universe, "Currency95"));
        }

        [Test]
        public void FindAll()
        {
            var universe = this.Locator.GetInstance<IUniverseManager>().Load(1);

            var manager = this.Locator.GetInstance<ICurrencyManager>();
            var currencies = manager.FindAll(universe);
            Assert.That(currencies, Is.Not.Null);
            Assert.That(currencies.Count(), Is.EqualTo(3));
        }

        [Test]
        public void Update()
        {
            var manager = this.Locator.GetInstance<ICurrencyManager>();
            var currency = manager.Load(1);
            currency.Name = this.ShortStringGenerator.GetRandomValue();
            currency.Description = this.ShortStringGenerator.GetRandomValue();
            currency.CurrencyCode = "abc";
            currency.Position = 19;
            currency.Hidden = true;
            manager.Update(currency);

            this.Session.Flush();
        }
        
        [Test]
        public void Delete()
        {
            var manager = this.Locator.GetInstance<ICurrencyManager>();
            var universe = this.Locator.GetInstance<IUniverseManager>().Load(1);

            var data = new CurrencyEntity { Name = this.ShortStringGenerator.GetRandomValue() };
            var instance = manager.Create(universe, data);
            this.Session.Flush();
            manager.Delete(instance);
        }

        /*[Test]
        public void IllegalDelete()
        {
            var manager = this.Locator.GetInstance<ICurrencyManager>();
            var currency = manager.Load(1);
            Assert.Throws<DeleteConstraintException>(() => manager.Delete(currency));
        }*/
    }
}