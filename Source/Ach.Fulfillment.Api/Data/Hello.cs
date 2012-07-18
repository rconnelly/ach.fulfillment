namespace Ach.Fulfillment.Api.Data
{
    using ServiceStack.ServiceHost;

    [RestService("/hello")]
    [RestService("/hello/{Name}")]
    public class Hello
    {
        public string Name { get; set; }
    }
}