namespace Ach.Fulfillment.Data.Common
{
    public abstract class SpecificationInstanceBase<TEntity> : SpecificationBase<TEntity>, IInstanceQueryData<TEntity>
    {
        public TEntity Instance { get; set; }
    }
}