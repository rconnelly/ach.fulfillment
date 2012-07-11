namespace Ach.Fulfillment.Data
{
    public class CurrencyEntity : BaseEntity
    {
        public virtual string Name { get; set; }
   
        public virtual string Description { get; set; }
   
        public virtual string CurrencyCode { get; set; }
   
        public virtual bool Hidden { get; set; }
   
        public virtual float Position { get; set; }

        public virtual UniverseEntity Universe { get; set; }
    }
}