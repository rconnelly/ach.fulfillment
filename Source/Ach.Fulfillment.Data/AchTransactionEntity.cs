namespace Ach.Fulfillment.Data
{
    public class AchTransactionEntity:BaseEntity
    {
        public virtual UserEntity User { get; set; }
        
        public virtual string MerchantId { get; set; }

        public virtual string MerchantName { get; set; }

        public virtual string MerchantDescription { get; set; }

        public virtual string CallbackUrl { get; set; }

        public virtual decimal Amount { get; set; }

        public virtual string AccountId { get; set; }

        public virtual string RoutingNumber { get; set; }

        public virtual bool IsQueued { get; set; }



    }
}
