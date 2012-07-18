namespace Ach.Fulfillment.Api.Data
{
    using ServiceStack.ServiceInterface.ServiceModel;

    public class HelloResponse
    {
        public ResponseStatus ResponseStatus { get; set; }

        public string Result { get; set; }
    }
}