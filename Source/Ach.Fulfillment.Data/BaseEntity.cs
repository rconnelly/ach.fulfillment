namespace Ach.Fulfillment.Data
{
    using System;

    public abstract class BaseEntity : IEntity
    {
        public virtual long Id { get; set; }
   
        public virtual DateTime Created { get; set; }
   
        public virtual DateTime? Modified { get; set; }
    }
}