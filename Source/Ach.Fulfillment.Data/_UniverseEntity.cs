namespace Ach.Fulfillment.Data
{
    using System;
    using System.Collections.Generic;

    [Obsolete]
    public class UniverseEntity : BaseEntity
    {
        public virtual string Login { get; set; }
   
        public virtual string PasswordHash { get; set; }
   
        public virtual bool Deleted { get; set; }

        public virtual ICollection<CurrencyEntity> Currencies { get; set; }
    }
}