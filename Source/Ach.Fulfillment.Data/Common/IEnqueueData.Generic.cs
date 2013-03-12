namespace Ach.Fulfillment.Data.Common
{
    public interface IEnqueueData<T> : IActionData<T>, IEnqueueData
    {
        new T Instance { get; }
    }
}