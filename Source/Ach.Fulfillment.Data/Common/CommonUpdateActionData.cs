namespace Ach.Fulfillment.Data.Common
{
    public class CommonUpdateActionData<TEntity> : IInstanceActionData<TEntity>
    {
        public TEntity Instance { get; set; }
    }
}