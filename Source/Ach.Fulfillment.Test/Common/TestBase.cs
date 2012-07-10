namespace Ach.Fulfillment.Test.Common
{
    using Ach.Fulfillment.Common;
    using Ach.Fulfillment.Initialization.Configuration;

    using NUnit.Framework;

    public abstract class TestBase
    {
        [TestFixtureSetUp]
        public virtual void Initialize()
        {
            NoCategoryTraceListener.Install();
            Shell.Start<InitializationContainerExtension>();
        }

        [TestFixtureTearDown]
        public virtual void Deinitialize()
        {
            Shell.Shutdown();
        }
    }
}