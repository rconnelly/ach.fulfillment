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
        public void UniverseSpecification()
        {
            new PersistenceSpecification<UniverseEntity>(this.Session)
                .CheckProperty(c => c.Login, this.ShortStringGenerator.GetRandomValue())
                .CheckProperty(c => c.PasswordHash, this.ShortStringGenerator.GetRandomValue())
            .VerifyTheMappings();
        }

        [Test]
        public void CurrencySpecification()
        {
            new PersistenceSpecification<CurrencyEntity>(this.Session)
                .CheckProperty(c => c.Name, this.ShortStringGenerator.GetRandomValue())
                .CheckProperty(c => c.Description, this.ShortStringGenerator.GetRandomValue())
                .CheckProperty(c => c.CurrencyCode, "RUB")
                .CheckProperty(c => c.Position, 1f)
                .CheckEntity(c => c.Universe, this.Session.Load<UniverseEntity>(1L))
            .VerifyTheMappings();
        }

        #endregion
    }
}