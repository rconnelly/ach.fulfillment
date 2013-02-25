namespace Ach.Fulfillment.Web.Areas.Api.Models
{
    using System;

    [Serializable]
    public class AchTransactionStatusModel
    {
        public long AchTransactionId { get; set; }

        public string Status { get; set; }
    }
}