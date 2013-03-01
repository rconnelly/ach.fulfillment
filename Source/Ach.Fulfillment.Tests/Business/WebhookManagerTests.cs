namespace Ach.Fulfillment.Tests.Business
{
    using Ach.Fulfillment.Business;
    using Ach.Fulfillment.Business.Impl;

    using NUnit.Framework;


    [TestFixture]
    public class WebhookManagerTests : BusinessIntegrationTestBase
    {
        [Test]
        public void CreateTest()
        {
            var manager = Locator.GetInstance<IWebhookManager>();
            var webhook = this.CreateTestWebhook();

            var instance = manager.Create(webhook);

            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.Id, Is.GreaterThan(0));
        }

        [Test]
        public void GetAllToSendTest()
        {
            var manager = Locator.GetInstance<WebhookManager>();
            var webhook = this.CreateTestWebhook();

            var instance = manager.Create(webhook);

            this.ClearSession(instance);

            var webhooks = manager.GetAllToSend();

            Assert.That(webhooks, Is.Not.Null);
            Assert.AreEqual(1, webhooks.Count);
        }

    }
}
