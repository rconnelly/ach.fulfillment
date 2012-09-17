using Ach.Fulfillment.Data;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace Ach.Fulfillment.Persistence.Impl.Mappings
{
    public class AchTransactionMapping : IAutoMappingOverride<AchTransactionEntity>
    {
        public void Override(AutoMapping<AchTransactionEntity> mapping)
        {
            mapping.References(x => x.User, "UserId").Cascade.None(); 
        }
    }
}
