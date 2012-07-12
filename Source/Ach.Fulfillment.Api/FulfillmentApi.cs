namespace Ach.Fulfillment.Api
{
    using System;
    using System.Globalization;

    using ServiceStack.ServiceHost;

    public class Test
    {
        public string Token { get; set; }
    }

    public class TestResponse
    {
        public string Response { get; set; }

        public string Token { get; set; }
    }

    public class FulfillmentApi : IService<Test>
    {
        public object Execute(Test request)
        {
            return new TestResponse
                {
                    Token = request.Token,
                    Response = DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture)
                };
        }
    }
}