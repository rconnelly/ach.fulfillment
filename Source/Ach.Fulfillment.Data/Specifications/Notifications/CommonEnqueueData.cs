namespace Ach.Fulfillment.Data.Specifications.Notifications
{
    using Ach.Fulfillment.Data.Common;

    public class CommonEnqueueData<TEntity> : IInstanceActionData<TEntity>, ICommonEnqueueData
    {
        public TEntity Instance { get; set; }

        object ICommonEnqueueData.Instance 
        { 
            get
            {
                return this.Instance;
            } 
        }
    }
}