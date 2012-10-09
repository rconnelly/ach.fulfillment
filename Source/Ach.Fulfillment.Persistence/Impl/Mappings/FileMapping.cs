using Ach.Fulfillment.Data;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace Ach.Fulfillment.Persistence.Impl.Mappings
{
    public class FileMapping:IAutoMappingOverride<FileEntity>
    {
        public void Override(AutoMapping<FileEntity> mapping)
        {
                 mapping.HasMany(i => i.Transactions)
                .Table("FileTransaction")
                .LazyLoad();

                 mapping.References(x => x.Partner, "PartnerId")
                     .Fetch.Join();
        }
    }
}
