namespace Ach.Fulfillment.Test
{
    using Ach.Fulfillment.Common;

    using Microsoft.Practices.ServiceLocation;

    using NUnit.Framework;

    [TestFixture]
    public class Main
    {
        [SetUp]
        public void Setup()
        {
            Shell.Start();
        }

        [Test]
        public void ServiceLocatorInitTest()
        {
            Assert.That(ServiceLocator.Current, Is.Not.Null);
        }

        [TearDown]
        public void TearDown()
        {
            Shell.Shutdown();
        }
    }
}
