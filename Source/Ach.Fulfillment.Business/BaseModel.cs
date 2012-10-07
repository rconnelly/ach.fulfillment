using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ach.Fulfillment.Data;

namespace Ach.Fulfillment.Business
{
    public class BaseModel<T>
        where T : BaseEntity
    {
        public T Model { get; set; }
    }
}
